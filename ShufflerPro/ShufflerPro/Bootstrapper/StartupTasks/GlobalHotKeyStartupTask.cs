using Bootstrap.Extensions.StartupTasks;
using ShufflerPro.Framework;

namespace ShufflerPro.Bootstrapper.StartupTasks;

public class GlobalHotKeyStartupTask : IStartupTask
{
    private readonly HotKeyListener _hotKeyListener;

    public GlobalHotKeyStartupTask(HotKeyListener hotKeyListener)
    {
        _hotKeyListener = hotKeyListener;
    }

    public void Run()
    {
        _hotKeyListener.Register();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}