using System.Reflection;
using Bootstrap;
using Bootstrap.Ninject;
using Ninject;
using ShufflerPro.Database.Interfaces;

namespace ShufflerPro.Database.Bootstrapper;

public static class BootstrapExtensions
{
    public static IIncludedAssemblies ShufflerProDatabase(this IIncludedAssemblies @this)
    {
        return @this.Including.Assembly(Assembly.GetExecutingAssembly());
    }
}

public class NinjectRegistration : INinjectRegistration
{
    public void Register(IKernel container)
    {
        var localDatabasePath = new DatabasePath();
        container.Bind<IDatabasePath>().ToConstant(localDatabasePath);
    }
}