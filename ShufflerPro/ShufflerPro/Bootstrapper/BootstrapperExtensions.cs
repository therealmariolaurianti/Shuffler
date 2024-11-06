using Ninject;
using NLog;

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
}