using System.Windows;
using Caliburn.Micro;
using Action = System.Action;

namespace ShufflerPro.Framework.WPF;

public abstract class ViewModelBase : Screen
{
    protected void RunAsync(Func<Task> func, Action? complete = null, Action<Exception>? error = null,
        Action? done = null)
    {
        var taskCompletion = new NotifyTaskCompletion(async () => await func(), complete, error, done);
        taskCompletion.Refresh();
    }

    protected override void OnViewAttached(object view, object context)
    {
        if (view is not FrameworkElement d)
            return;

        if (ScreenName.GetName(d) != null)
            DisplayName = ScreenName.GetName(d);

        base.OnViewAttached(view, context);
    }
}