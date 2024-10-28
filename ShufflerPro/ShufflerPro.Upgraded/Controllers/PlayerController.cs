using System.Windows.Threading;
using NAudio.Wave;
using ShufflerPro.Upgraded.Objects;

namespace ShufflerPro.Upgraded.Controllers;

public class PlayerController(WaveOutEvent outEvent, CancellationTokenSource cancellationToken)
    : IDisposable
{
    private AudioFileReader? _audioFileReader;
    private CancellationTokenSource? _cancellationToken = cancellationToken;
    private WaveOutEvent? _outEvent = outEvent;

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

    public bool PlaySong(Song song)
    {
        try
        {
            using (_audioFileReader = new AudioFileReader(song.Path))
            {
                _outEvent ??= new WaveOutEvent();

                _outEvent.Init(_audioFileReader);
                _outEvent.Play();

                DelayAction(_audioFileReader.TotalTime.Milliseconds, Dispose);
            }
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static void DelayAction(int millisecond, Action action)
    {
        var timer = new DispatcherTimer();

        timer.Tick += delegate
        {
            action.Invoke();
            timer.Stop();
        };

        timer.Interval = TimeSpan.FromSeconds(millisecond);
        timer.Start();
    }
}