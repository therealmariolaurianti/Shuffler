using System.Windows;
using Caliburn.Micro;
using Ninject;
using ShufflerPro.Upgraded.Screens.Shell;

namespace ShufflerPro.Upgraded.Bootstrapper;

public class AppBootstrapper : BootstrapperBase
{
    private readonly IKernel _kernel;

    public AppBootstrapper()
    {
        Initialize();

        _kernel = new StandardKernel();
    }

    protected override void OnExit(object sender, EventArgs e)
    {
        _kernel.Dispose();
        base.OnExit(sender, e);
    }

    protected override object GetInstance(Type service, string key)
    {
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        return _kernel.Get(service);
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
        return _kernel.GetAll(service);
    }

    protected override void BuildUp(object instance)
    {
        _kernel.Inject(instance);
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        _kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
        _kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();

        DisplayRootViewForAsync<ShellViewModel>();
    }
}