using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace ShufflerPro.Upgraded.Screens.Shell;

public class CountDownTimer : IDisposable
{
    private readonly Stopwatch _stopWatch = new();
    private readonly Timer _timer = new();
    private TimeSpan _max = TimeSpan.FromMilliseconds(30000);

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

    public TimeSpan TimeLeft => _max.TotalMilliseconds - _stopWatch.ElapsedMilliseconds > 0
        ? TimeSpan.FromMilliseconds(_max.TotalMilliseconds - _stopWatch.ElapsedMilliseconds)
        : TimeSpan.FromMilliseconds(0);

    private bool _mustStop => _max.TotalMilliseconds - _stopWatch.ElapsedMilliseconds < 0;
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
            _stopWatch.Stop();
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

    public void Start()
    {
        _timer.Start();
        _stopWatch.Start();
    }

    public void Pause()
    {
        _timer.Stop();
        _stopWatch.Stop();
    }

    public void Stop()
    {
        Reset();
        Pause();
    }

    public void Reset()
    {
        _stopWatch.Reset();
    }

    public void Restart()
    {
        _stopWatch.Reset();
        _timer.Start();
    }
}