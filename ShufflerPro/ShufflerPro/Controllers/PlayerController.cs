using System.Timers;
using NAudio.Wave;
using ShufflerPro.Client;
using ShufflerPro.Client.AudioEqualizer;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Framework;
using ShufflerPro.Framework.WPF.Controls.Visualizer;
using ShufflerPro.Result;

namespace ShufflerPro.Controllers;

public class PlayerController(
    WaveOutEvent outEvent,
    IEqualizerBandContainer equalizerBandContainer,
    RadioController radioController,
    ShufflerWindowManager windowManager) : IDisposable
{
    private AudioFileReader? _audioFileReader;
    private Equalizer? _equalizer;
    private bool _isPlayingStaticSong;
    private WaveOutEvent? _outEvent = outEvent;
    private ISongQueue? _songQueue;
    private PausableTimer? _timer;

    public required Action PlayerDisposed;
    public required Action<Song> SongChanged;

    public bool Playing => _outEvent?.PlaybackState == PlaybackState.Playing || radioController.IsPlaying;
    public bool IsCompleted { get; set; }
    public bool IsPaused { get; private set; }

    public bool IsPlayingStaticSong => radioController.IsPlaying;

    public void Dispose()
    {
        VisualizerEngine.Instance.Reset();
        
        _outEvent?.Stop();
        _outEvent?.Dispose();

        _audioFileReader?.Dispose();

        _timer?.Stop();
        _timer?.Dispose();

        _equalizer = null;
        _timer = null;
        _outEvent = null;
        _audioFileReader = null;

        PlayerDisposed.Invoke();
        
        GC.SuppressFinalize(this);
    }

    private NewResult<NewUnit> StartNextSong(ISongQueue? songQueue)
    {
        if (songQueue?.NextSong is null)
        {
            Dispose();
            return NewResultExtensions.CreateFail<NewUnit>("Player disposed.");
        }

        SongChanged.Invoke(songQueue.NextSong);
        return NewUnit.Default;
    }

    private void StartPreviousSong(ISongQueue songQueue)
    {
        if (songQueue.PreviousSong is null)
            Dispose();
        else
            SongChanged.Invoke(songQueue.PreviousSong!);
    }

    public void ReInitialize()
    {
        Dispose();

        _outEvent = new WaveOutEvent();
        IsCompleted = false;
    }

    public void Cancel()
    {
        ReInitialize();
    }

    public void PlaySong(ISongQueue? songQueue)
    {
        if (songQueue?.CurrentSong is null)
            return;

        _songQueue = songQueue;

        try
        {
            if (_songQueue.CurrentSong.IsStatic)
            {
                radioController.StartStation(_songQueue.CurrentSong.Path!);
                _isPlayingStaticSong = true;
            }
            else
            {
                _isPlayingStaticSong = false;
                radioController.StopStation();

                _audioFileReader = new AudioFileReader(songQueue.CurrentSong.Path);
                var inputStream = VisualizerEngine.Instance
                    .StartVisualizer(_audioFileReader, _songQueue.CurrentSong.Path!, false);

                _equalizer = new Equalizer(inputStream, equalizerBandContainer.Bands);

                _outEvent ??= new WaveOutEvent();
                _outEvent.Init(_equalizer);
                _outEvent.Play();

                VisualizerEngine.Instance.IsPlaying = true;

                DelayAction(songQueue.CurrentSong.Duration!.Value.TotalMilliseconds);
            }
        }
        catch (Exception e)
        {
            windowManager.ShowException(e);
        }
    }

    public void SetCurrentTime(TimeSpan time)
    {
        if (_audioFileReader is null)
            return;

        _audioFileReader.CurrentTime = time;

        var actual = (TimeSpan)(_songQueue?.CurrentSong!.Duration! - time);

        StopStart(actual);
    }

    public void DelayAction(double millisecond)
    {
        if (_timer != null)
        {
            if (_timer.Enabled)
                _timer.Stop();

            _timer.Elapsed -= OnTimerElapsed;
        }

        _timer = new PausableTimer(millisecond);
        _timer.Elapsed += OnTimerElapsed;

        _timer.Start();
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _timer?.Stop();
        StartNextSong(_songQueue);
    }

    public void StopStart(TimeSpan time)
    {
        _timer?.Stop();
        DelayAction(time.TotalMilliseconds);
    }

    public void Pause()
    {
        if (_isPlayingStaticSong)
        {
            radioController.StopStation();
        }
        else
        {
            VisualizerEngine.Instance.IsPlaying = false;
            _outEvent?.Pause();
            _timer?.Pause();
        }

        IsPaused = true;
    }

    public void Resume()
    {
        if (_isPlayingStaticSong)
        {
            radioController.StartStation(_songQueue!.CurrentSong!.Path!);
        }
        else
        {
            VisualizerEngine.Instance.IsPlaying = true;
            _outEvent?.Play();
            _timer?.Resume();
        }

        IsPaused = false;
    }

    public NewResult<NewUnit> Skip(ISongQueue songQueue)
    {
        return StartNextSong(songQueue);
    }

    public void Previous(ISongQueue songQueue)
    {
        StartPreviousSong(songQueue);
    }

    public void UpdateEqualizer()
    {
        _equalizer?.Update();
    }

    public void Stop()
    {
        Dispose();
    }
}