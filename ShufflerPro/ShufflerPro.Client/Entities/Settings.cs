using LiteDB;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Client.Entities;

public class Settings : ISettings
{
    public ObjectId Id { get; set; }
    public Guid ThemeId { get; set; }
    public bool IsDarkModeEnabled { get; set; }

    public void Update(ISettings settings)
    {
        Id = settings.Id;
        ThemeId = settings.ThemeId;
        IsDarkModeEnabled = settings.IsDarkModeEnabled;
    }
}