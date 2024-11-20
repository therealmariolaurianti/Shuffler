using Bootstrap.Ninject;
using Ninject;
using Ninject.Extensions.Conventions;
using ShufflerPro.Client.Radio;

namespace ShufflerPro.Client.Bootstrapper;

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