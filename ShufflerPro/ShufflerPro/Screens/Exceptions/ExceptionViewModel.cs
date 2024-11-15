using System.Diagnostics;
using System.IO;
using ShufflerPro.Client.Extensions;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Database;
using ShufflerPro.Framework.WPF;

namespace ShufflerPro.Screens.Exceptions;

public interface IExceptionViewModelFactory : IFactory
{
    ExceptionViewModel Create(Exception exception);
}

public class ExceptionViewModel : ViewModelBase
{
    private string _exceptionMessages;

    public ExceptionViewModel(Exception exception)
    {
        _exceptionMessages = string.Join(Environment.NewLine, exception.Messages());
    }

    public string ExceptionMessages
    {
        get => _exceptionMessages;
        set
        {
            if (value == _exceptionMessages) return;
            _exceptionMessages = value;
            NotifyOfPropertyChange();
        }
    }

    public void OpenLogFile()
    {
        RootFinder.FindRoot()
            .Do(root =>
            {
                var files = Directory.GetFiles(root, "*.log*", SearchOption.AllDirectories);
                var logFile = files.FirstOrDefault();
                if (logFile is not null)
                    Process.Start(new ProcessStartInfo(logFile)
                    {
                        UseShellExecute = true
                    });
            });
    }

    public void OpenGitHubIssues()
    {
        WebsiteLauncher.OpenWebsite("https://github.com/therealmariolaurianti/Shuffler/issues");
    }
}