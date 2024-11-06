using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Framework.WPF;

namespace ShufflerPro.Screens.Settings;

public interface ISettingsViewModelFactory : IFactory
{
    SettingsViewModel Create();
}

public class SettingsViewModel : ViewModelBase
{
    private bool _isDarkModeEnabled;
    private Theme _selectedTheme;

    public SettingsViewModel()
    {
        SelectedTheme = Themes.Default;
        IsDarkModeEnabled = true;

        DisplayName = "Settings";
    }

    public Theme SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (Equals(value, _selectedTheme)) return;
            _selectedTheme = value;
            NotifyOfPropertyChange();
            ThemeController.ChangeTheme(value, IsDarkModeEnabled);
        }
    }

    public bool IsDarkModeEnabled
    {
        get => _isDarkModeEnabled;
        set
        {
            if (value == _isDarkModeEnabled) return;
            _isDarkModeEnabled = value;
            NotifyOfPropertyChange();
            ThemeController.ChangeTheme(SelectedTheme, value);
        }
    }
}