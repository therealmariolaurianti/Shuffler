using Bootstrap.Extensions.StartupTasks;
using ShufflerPro.Framework;

namespace ShufflerPro.Bootstrapper.StartupTasks;

public class SettingsStartupTask : IStartupTask
{
    private readonly SettingsLoader _settingsLoader;

    public SettingsStartupTask(SettingsLoader settingsLoader)
    {
        _settingsLoader = settingsLoader;
    }

    public void Run()
    {
        Task.Run(async () => await _settingsLoader.Load());
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}