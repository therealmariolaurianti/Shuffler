using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace ShufflerPro.Upgraded.Screens.Shell;

public class CountDownTimer : IDisposable
{
    private readonly Timer _timer = new();
    private TimeSpan _max = TimeSpan.FromMilliseconds(30000);
    public Stopwatch _stpWatch = new();

    public Action? CountDownFinished;
    public Action? TimeChanged;

    public CountDownTimer(TimeSpan ts)
    {
        SetTime(ts);
        Init();
    }

    public CountDownTimer()
    {
        Init();
    }

    public double StepMs
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    public TimeSpan TimeLeft => _max.TotalMilliseconds - _stpWatch.ElapsedMilliseconds > 0
        ? TimeSpan.FromMilliseconds(_max.TotalMilliseconds - _stpWatch.ElapsedMilliseconds)
        : TimeSpan.FromMilliseconds(0);

    private bool _mustStop => _max.TotalMilliseconds - _stpWatch.ElapsedMilliseconds < 0;

    public string TimeLeftStr => TimeLeft.ToString("mm':'ss");
    public bool IsRunning => _timer.Enabled;

    public void Dispose()
    {
        _timer.Dispose();
    }

    private void TimerTick(object? sender, EventArgs e)
    {
        TimeChanged?.Invoke();

        if (_mustStop)
        {
            CountDownFinished?.Invoke();
            _stpWatch.Stop();
            _timer.Enabled = false;
        }
    }

    private void Init()
    {
        StepMs = 1000;
        _timer.Elapsed += TimerTick;
    }

    public void SetTime(TimeSpan ts)
    {
        _max = ts;
        TimeChanged?.Invoke();
    }

    public void SetTime(int min, int sec = 0)
    {
        SetTime(TimeSpan.FromSeconds(min * 60 + sec));
    }

    public void Start()
    {
        _timer.Start();
        _stpWatch.Start();
    }

    public void Pause()
    {
        _timer.Stop();
        _stpWatch.Stop();
    }

    public void Stop()
    {
        Reset();
        Pause();
    }

    public void Reset()
    {
        _stpWatch.Reset();
    }

    public void Restart()
    {
        _stpWatch.Reset();
        _timer.Start();
    }
}