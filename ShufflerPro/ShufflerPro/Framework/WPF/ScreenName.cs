using System.Windows;

namespace ShufflerPro.Framework.WPF;

public class ScreenName
{
    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.RegisterAttached("DisplayName", typeof(string),
            typeof(ScreenName), new PropertyMetadata(null));

    public static string? GetName(DependencyObject obj)
    {
        return (string?)obj.GetValue(DisplayNameProperty);
    }

    public static void SetName(DependencyObject obj, string value)
    {
        obj.SetValue(DisplayNameProperty, value);
    }
}