using System.Windows;
using System.Windows.Controls;
using ShufflerPro.Client.Entities;
using ShufflerPro.Framework.WPF.Objects;

namespace ShufflerPro.Framework.WPF.Controls;

public class ThemeComboBox : ComboBox
{
    public static readonly DependencyProperty ThemeIdProperty = DependencyProperty.Register(
        nameof(ThemeId), typeof(Guid), typeof(ThemeComboBox),
        new FrameworkPropertyMetadata(default(Guid), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnThemeIdChanged));

    private readonly List<Theme> _itemsSource;

    public ThemeComboBox()
    {
        SetResourceReference(StyleProperty, typeof(ComboBox));

        _itemsSource = Themes.Items.ToList();
        
        DisplayMemberPath = nameof(Theme.Name);
        ItemsSource = _itemsSource;

        SelectionChanged += OnSelectedItemChanged;
    }

    private void OnSelectedItemChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedItem is Theme theme)
        {
            ThemeId = theme.Id;
        }
    }

    public Guid ThemeId
    {
        get => (Guid)GetValue(ThemeIdProperty);
        set => SetValue(ThemeIdProperty, value);
    }

    private static void OnThemeIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ThemeComboBox themeComboBox)
            themeComboBox.SetSelectedItem();
    }

    private void SetSelectedItem()
    {
        SelectedItem = _itemsSource.Single(i => i.Id == ThemeId);
    }
}