using Bootstrap.Extensions.StartupTasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ShufflerPro.Bootstrapper.StartupTasks;

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