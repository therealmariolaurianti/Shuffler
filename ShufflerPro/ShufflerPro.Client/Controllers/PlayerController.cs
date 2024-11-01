using NAudio.Wave;
using ShufflerPro.Client.Entities;
using Timer = System.Timers.Timer;

namespace ShufflerPro.Client.Controllers;

public class PlayerController(WaveOutEvent outEvent) : IDisposable
{
    private AudioFileReader? _audioFileReader;
    private WaveOutEvent? _outEvent = outEvent;
    private Timer? _timer;

    public required Action<Song> SongChanged;
    public required Action PlayerDisposed;

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

    private void StartNextSong(SongQueue songQueue)
    {
        if (songQueue.NextSong is null)
            Dispose();
        else
            SongChanged.Invoke(songQueue.NextSong);
    }
    
    private void StartPreviousSong(SongQueue songQueue)
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

    public void PlaySong(SongQueue songQueue)
    {
        if (songQueue.CurrentSong is null)
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
        _timer = new Timer();

        _timer.Elapsed += delegate
        {
            _timer.Stop();
            action.Invoke();
        };

        _timer.Interval = millisecond;
        _timer.Start();
    }

    public void Pause()
    {
        if (Playing)
        {
            _outEvent?.Pause();
            _timer?.Stop();
            IsPaused = true;
        }
    }

    public void Resume()
    {
        if (!Playing)
        {
            _outEvent?.Play();
            _timer?.Start();
            IsPaused = false;
        }
    }

    public void Skip(SongQueue songQueue)
    {
        StartNextSong(songQueue);
    }

    public void Previous(SongQueue songQueue, double elapsedRunningTime)
    {
        if (elapsedRunningTime >= 5)
            SongChanged.Invoke(songQueue.CurrentSong!);
        else
            StartPreviousSong(songQueue);
    }
}