namespace ShufflerPro.Framework.WPF.Windows;

// The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
// what value of the enum to set.
// Copied from dwmapi.h
public enum DWM_WINDOW_CORNER_PREFERENCE
{
    DWMWCP_DEFAULT      = 0,
    DWMWCP_DONOTROUND   = 1,
    DWMWCP_ROUND        = 2,
    DWMWCP_ROUNDSMALL   = 3
}