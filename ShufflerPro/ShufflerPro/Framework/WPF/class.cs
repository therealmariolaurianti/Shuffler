using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;

namespace ShufflerPro.Framework.WPF;

public class CloseWindowCommand : ICommand
{
    public static readonly ICommand Instance = new CloseWindowCommand();

    private CloseWindowCommand()
    {
    }

    public bool CanExecute(object? parameter)
    {
        return parameter is Window;
    }

    public event EventHandler? CanExecuteChanged;

    public void Execute(object? parameter)
    {
        if (CanExecute(parameter))
        {
            if (parameter is IScreen screen)
                screen.TryCloseAsync(false);
            else
                ((Window)parameter!).Close();
        }
    }

    protected virtual void OnCanExecuteChanged()
    {
        var handler = CanExecuteChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }
}