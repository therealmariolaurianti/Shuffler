using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;

namespace ShufflerPro.Framework;

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
        return await _databaseController
            .LoadSettings()
            .Do(settings =>
            {
                _settings.Update(settings);
                ThemeManager.ChangeTheme(settings.ThemeId, settings.IsDarkModeEnabled);
            })
            .Map(_ => NewUnit.Default);
    }
}