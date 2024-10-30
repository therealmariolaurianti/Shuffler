using NAudio.Wave;
using ShufflerPro.Client.Entities;
using Timer = System.Timers.Timer;

namespace ShufflerPro.Client.Controllers;

public class PlayerController(WaveOutEvent outEvent, CancellationTokenSource cancellationToken)
    : IDisposable
{
    private AudioFileReader? _audioFileReader;
    private CancellationTokenSource? _cancellationToken = cancellationToken;
    private WaveOutEvent? _outEvent = outEvent;

    public Action<Song> SongChanged;
    private static Timer _timer;

    public bool Playing => _outEvent?.PlaybackState == PlaybackState.Playing;
    public bool IsCompleted { get; set; }

    public void Dispose()
    {
        _outEvent?.Dispose();
        _audioFileReader?.Dispose();
        _cancellationToken?.Dispose();

        _outEvent = null;
        _audioFileReader = null;
        _cancellationToken = null;
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
        _cancellationToken = new CancellationTokenSource();
        IsCompleted = false;
    }

    public void Cancel()
    {
        _cancellationToken?.Cancel();
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

    public static void DelayAction(double millisecond, Action action)
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
            _timer.Stop();
            return;
        }

        if (!Playing)
            try
            {
                _outEvent?.Play();
                _timer.Start();
            }
            catch (Exception)
            {
                //ignored
            }
    }
}