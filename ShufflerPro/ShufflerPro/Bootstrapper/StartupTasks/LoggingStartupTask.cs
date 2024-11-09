using Bootstrap.Extensions.StartupTasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using ShufflerPro.Framework.WPF;

namespace ShufflerPro.Bootstrapper.StartupTasks;

public class ThemeStartupTask : IStartupTask
{
    public void Run()
    {
        //ThemeManager.ChangeTheme(value, IsDarkModeEnabled);
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}

public class LoggingStartupTask : IStartupTask
{
    public void Run()
    {
        var config = new LoggingConfiguration();
        var logfile = new FileTarget("logfile") { FileName = "Exceptions.log" };

        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

        LogManager.Configuration = config;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}