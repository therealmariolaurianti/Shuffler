namespace ShufflerPro.Client.Interfaces;

public interface ISettings
{
    public Guid ThemeId { get; set; }
    public bool IsDarkModeEnabled { get; set; }
}