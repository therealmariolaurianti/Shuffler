using System.Windows;
using ShufflerPro.Framework.WPF.Objects;
using Theme = ShufflerPro.Client.Entities.Theme;

namespace ShufflerPro.Framework.WPF;

public static class ThemeManager
{
    public static void ChangeTheme(Guid themeId, bool isDarkModeEnabled)
    {
        var theme = Themes.Items.Single(i => i.Id == themeId);
        var lightOrDark = isDarkModeEnabled ? "dark" : "light";
        var newTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme($"{lightOrDark}.{theme.Name}");
        if (newTheme != null)
            ControlzEx.Theming.ThemeManager.Current.ChangeTheme(Application.Current, newTheme);
    }
}