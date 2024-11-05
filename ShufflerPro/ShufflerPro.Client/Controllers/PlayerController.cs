using NAudio.Wave;
using ShufflerPro.Client.Entities;
using Timer = System.Timers.Timer;

namespace ShufflerPro.Client.Controllers;

public class PlayerController(WaveOutEvent outEvent) : IDisposable
{
    private AudioFileReader? _audioFileReader;
    private WaveOutEvent? _outEvent = outEvent;
    private PausableTimer? _timer;

    public required Action PlayerDisposed;
    public required Action<Song> SongChanged;

    public bool Playing => _outEvent?.PlaybackState == PlaybackState.Playing;
    public bool IsCompleted { get; set; }
    public bool IsPaused { get; private set; }

    public void Dispose()
    {
        _outEvent?.Stop();
        _outEvent?.Dispose();

        _audioFileReader?.Dispose();

        _timer?.Stop();
        _timer?.Dispose();

        _timer = null;
        _outEvent = null;
        _audioFileReader = null;

        PlayerDisposed.Invoke();
    }

    private void StartNextSong(ISongQueue songQueue)
    {
        if (songQueue.NextSong is null)
            Dispose();
        else
            SongChanged.Invoke(songQueue.NextSong);
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

        try
        {
            using (_audioFileReader = new AudioFileReader(songQueue.CurrentSong.Path))
            {
                _outEvent ??= new WaveOutEvent();

                _outEvent.Init(_audioFileReader);
                _outEvent.Play();

                DelayAction(songQueue.CurrentSong.Duration!.Value.TotalMilliseconds, () => StartNextSong(songQueue));
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public void DelayAction(double millisecond, Action action)
    {
        _timer = new PausableTimer(millisecond);

        _timer.Elapsed += delegate
        {
            _timer.Stop();
            action.Invoke();
        };
        
        _timer.Start();
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

    public void Skip(ISongQueue songQueue)
    {
        StartNextSong(songQueue);
    }

    public void Previous(ISongQueue songQueue)
    {
        StartPreviousSong(songQueue);
    }
}