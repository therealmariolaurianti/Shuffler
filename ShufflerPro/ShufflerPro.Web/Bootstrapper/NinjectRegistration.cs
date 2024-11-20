using System.Reflection;
using Bootstrap;
using Bootstrap.Ninject;
using Ninject;
using Ninject.Extensions.Conventions;
using ShufflerPro.Web.Radio;

namespace ShufflerPro.Web.Bootstrapper;

public class NinjectRegistration : INinjectRegistration
{
    public void Register(IKernel container)
    {
        container.Bind(x => x.FromThisAssembly()
            .SelectAllClasses()
            .InheritedFrom<IRadioStation>()
            .BindAllInterfaces());
    }
}

public static class BootstrapExtensions
{
    public static IIncludedAssemblies ShufflerProWeb(this IIncludedAssemblies @this)
    {
        return @this.Including.Assembly(Assembly.GetExecutingAssembly());
    }
}