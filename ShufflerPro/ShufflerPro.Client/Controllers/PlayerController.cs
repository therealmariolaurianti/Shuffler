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
    }

    private void OnSongComplete(Song currentSong, List<Song> songs)
    {
        var index = songs.IndexOf(currentSong) + 1;
        var nextTrack = songs.Skip(index).FirstOrDefault();

        if (nextTrack is null)
            Dispose();
        else
            SongChanged.Invoke(nextTrack);
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

    public bool PlaySong(Song song, List<Song> songs)
    {
        try
        {
            using (_audioFileReader = new AudioFileReader(song.Path))
            {
                _outEvent ??= new WaveOutEvent();

                _outEvent.Init(_audioFileReader);
                _outEvent.Play();

                DelayAction(song.Duration!.Value.TotalMilliseconds, () => OnSongComplete(song, songs));
            }
        }
        catch (Exception)
        {
            return false;
        }

        return true;
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

    public void PlayPause()
    {
        if (Playing)
        {
            _outEvent?.Pause();
            _timer?.Stop();
            IsPaused = true;
            return;
        }

        if (!Playing)
            try
            {
                _outEvent?.Play();
                _timer?.Start();
                IsPaused = false;
            }
            catch (Exception)
            {
                //ignored
            }
    }
}