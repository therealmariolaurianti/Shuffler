using Bootstrap.Extensions.StartupTasks;
using ShufflerPro.Result;
using ShufflerPro.Updates;
using ShufflerPro.Web;

namespace ShufflerPro.Bootstrapper.StartupTasks;

public class CheckForUpdateStartupTask(UpdateController updateController, IUpdateStatus updateStatus) : IStartupTask
{
    public void Run()
    {
        Task.Run(async () => await updateController.CheckForUpdate()
            .Do(isUpdateAvailable => updateStatus.IsUpdateAvailable = isUpdateAvailable));
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}