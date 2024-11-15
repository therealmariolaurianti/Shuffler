using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using NLog;
using ShufflerPro.Client.Extensions;
using ShufflerPro.Database;
using ShufflerPro.Framework.WPF;

namespace ShufflerPro.Screens.Exceptions;

public class ExceptionViewModel : ViewModelBase
{
    private string _exceptionMessages;

    public ExceptionViewModel(Exception exception, ILogger logger)
    {
        //log exception to file
        logger.Error(exception);
        
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

    [UsedImplicitly]
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

    [UsedImplicitly]
    public void OpenGitHubIssues()
    {
        WebsiteLauncher.OpenWebsite("https://github.com/therealmariolaurianti/Shuffler/issues");
    }

    [UsedImplicitly]
    public void Close()
    {
        TryCloseAsync(true);
    }
}