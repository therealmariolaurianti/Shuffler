using ShufflerPro.Client.Entities;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class SettingsController
{
    private readonly DatabaseController _databaseController;

    public SettingsController(DatabaseController databaseController)
    {
        _databaseController = databaseController;
    }

    public async Task<NewResult<NewUnit>> Update(ItemTracker<Settings> itemTracker)
    {
        if (itemTracker.PropertyDifferences.Count == 0)
            return NewUnit.Default;

        return await _databaseController.UpdateSettings(itemTracker.Item);
    }
}