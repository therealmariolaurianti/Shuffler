using Ninject;
using NLog;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Bootstrapper;

public static class BootstrapperExtensions
{
    public static void BindLogging(this IKernel container)
    {
        container.Bind<ILogger>().ToMethod(p =>
        {
            var logger = LogManager
                .GetLogger(p.Request.Target?.Member.DeclaringType?.FullName ?? typeof(App).FullName);
            return logger;
        });
    }

    public static void BindSettings(this IKernel container)
    {
        container.Bind<ISettings>().ToMethod(_ => container.Get<DatabaseController>().LoadSettings());
    }
}