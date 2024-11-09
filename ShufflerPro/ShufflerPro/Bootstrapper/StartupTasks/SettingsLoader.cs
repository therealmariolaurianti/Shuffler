using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Result;

namespace ShufflerPro.Bootstrapper.StartupTasks;

public class SettingsLoader
{
    private readonly DatabaseController _databaseController;
    private readonly SettingsContainer _settings;

    public SettingsLoader(DatabaseController databaseController, SettingsContainer settingsContainer)
    {
        _databaseController = databaseController;
        _settings = settingsContainer;
    }

    public async Task<NewResult<NewUnit>> Load()
    {
        return (await _databaseController
                .LoadSettings()
                .Do(settings => _settings.Update(settings)))
            .Convert<NewUnit>();
    }
}