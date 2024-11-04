using Bootstrap.Ninject;
using Ninject;
using ShufflerPro.Client.Interfaces;
using Ninject.Extensions.Conventions;

namespace ShufflerPro.Upgraded.Bootstrapper;

public class NinjectRegistration : INinjectRegistration
{
    public void Register(IKernel container)
    {
        container.Bind(x => x.FromThisAssembly()
            .SelectAllInterfaces()
            .InheritedFrom<IFactory>()
            .BindToFactory());
    }
}