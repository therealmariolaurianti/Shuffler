using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ShufflerPro.Upgraded.Framework;

public class UiServices
{
    private static bool _isBusy;

    public static Action SetBusyState = () => InternalSetBusyState(true);

    private static void InternalSetBusyState(bool busy)
    {
        if (busy == _isBusy)
            return;
        _isBusy = busy;
        Mouse.OverrideCursor = busy ? Cursors.Wait : null;

        if (_isBusy)
            // ReSharper disable once ObjectCreationAsStatement
            new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle,
                dispatcherTimer_Tick, Application.Current.Dispatcher);
    }

    private static void dispatcherTimer_Tick(object sender, EventArgs e)
    {
        var dispatcherTimer = sender as DispatcherTimer;
        if (dispatcherTimer != null)
        {
            InternalSetBusyState(false);
            dispatcherTimer.Stop();
        }
    }
}