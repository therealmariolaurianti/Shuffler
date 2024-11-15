using Bootstrap.Extensions.StartupTasks;
using ShufflerPro.Client.AudioEqualizer;

namespace ShufflerPro.Bootstrapper.StartupTasks;

public class EqualizerBandsStartUpTask : IStartupTask
{
    private readonly EqualizerBandContainer _equalizerBandContainer;

    public EqualizerBandsStartUpTask(EqualizerBandContainer equalizerBandContainer)
    {
        _equalizerBandContainer = equalizerBandContainer;
    }

    public void Run()
    {
        _equalizerBandContainer.Initialize();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}