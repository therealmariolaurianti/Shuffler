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
        var localDatabasePath = new DatabasePath(Directory.GetCurrentDirectory());
        localDatabasePath.Start()
            .IfSuccess(_ =>
            {
                container.Bind<IDatabasePath>().ToConstant(localDatabasePath);
            })
            .IfFail(_ => throw new ApplicationException(_.Message));
    }
}