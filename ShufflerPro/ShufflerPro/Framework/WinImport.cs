using System.Runtime.InteropServices;
using ShufflerPro.Framework.WPF.Windows;

namespace ShufflerPro.Framework;

public static partial class WinImport
{
    [LibraryImport("winmm.dll")]
    public static partial int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

    [LibraryImport("winmm.dll")]
    public static partial int waveOutSetVolume(IntPtr hwo, uint dwVolume);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnregisterHotKey(IntPtr hWnd, int id);
    
    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
        DWMWINDOWATTRIBUTE attribute,
        ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
        uint cbAttribute);
}