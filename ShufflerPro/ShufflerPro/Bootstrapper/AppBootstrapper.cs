using System.Windows;
using System.Windows.Threading;
using Bootstrap.AutoMapper;
using Bootstrap.Extensions.StartupTasks;
using Bootstrap.Ninject;
using Caliburn.Micro;
using Ninject;
using ShufflerPro.Bootstrapper.Extensions;
using ShufflerPro.Database.Bootstrapper;
using ShufflerPro.Framework;
using ShufflerPro.Screens.Exceptions;
using ShufflerPro.Screens.Startup;
using ShufflerPro.Web.Bootstrapper;

namespace ShufflerPro.Bootstrapper;

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

    protected override async void OnStartup(object sender, StartupEventArgs e)
    {
        _kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
        _kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
        _kernel.Bind<HotKeyListener>().ToSelf().InSingletonScope();

        _kernel.BindLogging();
        _kernel.BindSettings();
        _kernel.BindEqualizer();
        _kernel.BindAccessTokens();

        Bootstrap.Bootstrapper
            .Including.ShufflerProDatabase()
            .Including.ShufflerProWeb()
            .With.Ninject().WithContainer(_kernel)
            .With.StartupTasks()
            .With.AutoMapper()
            .Start();

        await DisplayRootViewForAsync<StartupViewModel>();
    }

    protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var exceptionViewModel = _kernel.Get<IExceptionViewModelFactory>();
        var windowManager = _kernel.Get<IWindowManager>();

        Execute.OnUIThread(() => windowManager.ShowDialogAsync(exceptionViewModel.Create(e.Exception)));
        Application.Shutdown();
    }
}