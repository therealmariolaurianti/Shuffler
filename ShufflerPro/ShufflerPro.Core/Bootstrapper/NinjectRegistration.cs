using Bootstrap.Ninject;
using Ninject;
using Ninject.Extensions.Conventions;
using ShufflerPro.Core.Interfaces;

namespace ShufflerPro.Core.Bootstrapper
{
    public class NinjectRegistration : INinjectRegistration
    {
        public void Register(IKernel container)
        {
            container.Bind(x => x.FromThisAssembly()
                .SelectAllInterfaces().InheritedFrom<IFactory>()
                .BindToFactory());
        }
    }
}