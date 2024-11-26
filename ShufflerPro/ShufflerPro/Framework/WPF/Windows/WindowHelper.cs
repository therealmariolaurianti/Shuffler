using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ShufflerPro.Framework.WPF.Windows;

public static class WindowHelper
{
    public static void WindowsRoundCorners(Window window)
    {
#if WINDOWS7_0_OR_GREATER
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            var hWnd = new WindowInteropHelper(window).EnsureHandle();
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;

            WinImport.DwmSetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref preference,
                sizeof(uint));
        }), DispatcherPriority.Background);
#endif
    }
}