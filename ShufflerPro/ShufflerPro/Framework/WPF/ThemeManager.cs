using System.Windows;
using Theme = ShufflerPro.Client.Entities.Theme;

namespace ShufflerPro.Framework.WPF;

public static class ThemeManager
{
    public static void ChangeTheme(Theme theme, bool isDarkModeEnabled)
    {
        var lightOrDark = isDarkModeEnabled ? "dark" : "light";
        var newTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme($"{lightOrDark}.{theme.Name}");
        if (newTheme != null)
            ControlzEx.Theming.ThemeManager.Current.ChangeTheme(Application.Current, newTheme);
    }
}