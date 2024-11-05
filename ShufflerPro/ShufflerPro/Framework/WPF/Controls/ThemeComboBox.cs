using System.Windows.Controls;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Framework.WPF.Controls;

public class ThemeComboBox : ComboBox
{
    public ThemeComboBox()
    {
        SetResourceReference(StyleProperty, typeof(ComboBox));

        DisplayMemberPath = nameof(Theme.Name);
        ItemsSource = Themes.Items.ToList();
    }
}