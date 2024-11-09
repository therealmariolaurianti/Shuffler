using LiteDB;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Database;

namespace ShufflerPro.Client.Entities;

public class Settings : ISettings
{
    public ObjectId Id { get; set; }
    public Guid ThemeId { get; set; }
    public bool IsDarkModeEnabled { get; set; }
}