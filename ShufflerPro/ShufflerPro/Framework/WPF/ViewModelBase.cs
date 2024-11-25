using System.Windows;
using Caliburn.Micro;
using ShufflerPro.Client.Attributes;
using ShufflerPro.Framework.WPF.AttachedProperties;
using Action = System.Action;

namespace ShufflerPro.Framework.WPF;

public abstract class ViewModelBase : Screen
{
    protected void RunAsync(Func<Task> func, Action? complete = null, Action<Exception>? error = null,
        Action? done = null, bool showIsBusy = true)
    {
        if(showIsBusy)
            UIServices.SetBusyState();
        
        var taskCompletion = new NotifyTaskCompletion(async () => await func(), complete, error, done);
        taskCompletion.Refresh();
    }

    protected override void OnViewAttached(object view, object context)
    {
        if (view is not FrameworkElement d)
            return;

        var displayName = ScreenNameAttachedProperty.GetName(d);
        if (displayName != null)
            DisplayName = displayName;

        base.OnViewAttached(view, context);
    }

    [IgnoreTracking]
    public override string? DisplayName { get; set; }
}