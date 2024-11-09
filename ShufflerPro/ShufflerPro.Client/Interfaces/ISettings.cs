using LiteDB;

namespace ShufflerPro.Client.Interfaces;

public interface ISettings
{
    public ObjectId Id { get; set; }
    public Guid ThemeId { get; set; }
    public bool IsDarkModeEnabled { get; set; }
    void Update(ISettings settings);
}