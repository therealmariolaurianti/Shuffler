using System.Windows;
using ControlzEx.Theming;
using Theme = ShufflerPro.Client.Entities.Theme;


namespace ShufflerPro.Upgraded.Framework.WPF;

public static class ThemeController
{
    public static void ChangeTheme(Theme theme)
    {
        var newTheme = ThemeManager.Current.GetTheme($"dark.{theme.Name}");
        if (newTheme != null)
            ThemeManager.Current.ChangeTheme(Application.Current, newTheme);
    }
}