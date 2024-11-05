using System.Windows;
using ControlzEx.Theming;
using Theme = ShufflerPro.Client.Entities.Theme;


namespace ShufflerPro.Framework.WPF;

public static class ThemeController
{
    public static void ChangeTheme(Theme theme, bool isDarkModeEnabled)
    {
        var lightOrDark = isDarkModeEnabled ? "dark" : "light";
        var newTheme = ThemeManager.Current.GetTheme($"{lightOrDark}.{theme.Name}");
        if (newTheme != null)
            ThemeManager.Current.ChangeTheme(Application.Current, newTheme);
    }
}