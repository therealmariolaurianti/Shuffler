﻿using System.Windows;
using System.Windows.Threading;
using Bootstrap.AutoMapper;
using Bootstrap.Extensions.StartupTasks;
using Bootstrap.Ninject;
using Caliburn.Micro;
using Ninject;
using ShufflerPro.Database.Bootstrapper;
using ShufflerPro.Screens.Startup;

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

        Bootstrap.Bootstrapper
            .Including.ShufflerProDatabase()
            .With.Ninject().WithContainer(_kernel)
            .With.StartupTasks()
            .With.AutoMapper()
            .Start();

        await DisplayRootViewForAsync<StartupViewModel>();
    }

    protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Execute.OnUIThread(() => MessageBox.Show("An unexpected error has occurred."));
        Application.Shutdown();
    }
}