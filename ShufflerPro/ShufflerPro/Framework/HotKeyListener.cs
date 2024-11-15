using System.Windows.Forms;

namespace ShufflerPro.Framework;

public class HotKeyListener : NativeWindow, IDisposable
{
    private const int VK_MEDIA_NEXT_TRACK = 0xB0;
    private const int VK_MEDIA_PREV_TRACK = 0xB1;
    private const int VK_MEDIA_PLAY_PAUSE = 0xB3;
    private const int VK_VOLUME_MUTE = 0xAD;

    private const int MOD_NONE = 0x0000;

    private const int HOTKEY_NEXT = 1;
    private const int HOTKEY_PREVIOUS = 2;
    private const int HOTKEY_PLAY_PAUSE = 3;
    private const int HOTKEY_MUTE = 4;
    private const int WM_HOTKEY = 0x0312;

    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            WinImport.UnregisterHotKey(Handle, HOTKEY_NEXT);
            WinImport.UnregisterHotKey(Handle, HOTKEY_PREVIOUS);
            WinImport.UnregisterHotKey(Handle, HOTKEY_PLAY_PAUSE);
            WinImport.UnregisterHotKey(Handle, HOTKEY_MUTE);

            DestroyHandle();
            _disposed = true;
        }
    }

    ~HotKeyListener()
    {
        Dispose(false);
    }

    public void Register()
    {
        CreateHandle(new CreateParams());

        WinImport.RegisterHotKey(Handle, HOTKEY_NEXT, MOD_NONE, VK_MEDIA_NEXT_TRACK);
        WinImport.RegisterHotKey(Handle, HOTKEY_PREVIOUS, MOD_NONE, VK_MEDIA_PREV_TRACK);
        WinImport.RegisterHotKey(Handle, HOTKEY_PLAY_PAUSE, MOD_NONE, VK_MEDIA_PLAY_PAUSE);
        WinImport.RegisterHotKey(Handle, HOTKEY_MUTE, MOD_NONE, VK_VOLUME_MUTE);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY)
        {
            var hotkeyId = m.WParam.ToInt32();
            switch (hotkeyId)
            {
                case HOTKEY_NEXT:
                    OnNextTrack?.Invoke();
                    break;
                case HOTKEY_PREVIOUS:
                    OnPreviousTrack?.Invoke();
                    break;
                case HOTKEY_PLAY_PAUSE:
                    OnPlayPause?.Invoke();
                    break;
                case HOTKEY_MUTE:
                    OnMute?.Invoke();
                    break;
            }
        }

        base.WndProc(ref m);
    }

    public event Action? OnNextTrack;
    public event Action? OnPreviousTrack;
    public event Action? OnPlayPause;
    public event Action? OnMute;
}