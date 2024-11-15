using System.Timers;
using NAudio.Wave;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class PlayerController(WaveOutEvent outEvent) : IDisposable
{
    private WaveOutEvent? _outEvent = outEvent;
    private ISongQueue _songQueue;
    private PausableTimer? _timer;

    public required Action PlayerDisposed;
    public required Action<Song> SongChanged;

    public bool Playing => _outEvent?.PlaybackState == PlaybackState.Playing;
    public bool IsCompleted { get; set; }
    public bool IsPaused { get; private set; }

    public AudioFileReader? AudioFileReader { get; set; }

    public void Dispose()
    {
        _outEvent?.Stop();
        _outEvent?.Dispose();

        AudioFileReader?.Dispose();

        _timer?.Stop();
        _timer?.Dispose();

        _timer = null;
        _outEvent = null;
        AudioFileReader = null;

        PlayerDisposed.Invoke();
    }

    private NewResult<NewUnit> StartNextSong(ISongQueue songQueue)
    {
        if (songQueue.NextSong is null)
        {
            Dispose();
            return NewResultExtensions.CreateFail<NewUnit>("Player disposed.");
        }

        SongChanged.Invoke(songQueue.NextSong);
        return NewUnit.Default;
    }

    private void StartPreviousSong(ISongQueue songQueue)
    {
        if (songQueue.NextSong is null)
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
            AudioFileReader = new AudioFileReader(songQueue.CurrentSong.Path);
            _outEvent ??= new WaveOutEvent();

            _outEvent.Init(AudioFileReader);
            _outEvent.Play();

            DelayAction(songQueue.CurrentSong.Duration!.Value.TotalMilliseconds);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public void SetCurrentTime(TimeSpan time)
    {
        if (AudioFileReader is null)
            return;

        AudioFileReader.CurrentTime = time;

        var actual = (TimeSpan)(_songQueue.CurrentSong!.Duration! - time);

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
        _outEvent?.Pause();
        _timer?.Pause();
        IsPaused = true;
    }

    public void Resume()
    {
        _outEvent?.Play();
        _timer?.Resume();
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
}