using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ShufflerPro.Framework.WPF.Windows;

public static class WindowHelper
{
    public static void WindowsRoundCorners(Window? window)
    {
        if (window == null || IsNotWindowsEleven())
            return;

        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            if (!window.IsActive)
                return;

            var hWnd = new WindowInteropHelper(window).EnsureHandle();
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;

            WinImport.DwmSetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref preference,
                sizeof(uint));
        }), DispatcherPriority.Background);
    }

    private static bool IsNotWindowsEleven()
    {
        var version = Environment.OSVersion.Version;

        return version.Major switch
        {
            10 when version.Build >= 22000 => false,
            _ => true
        };
    }
}