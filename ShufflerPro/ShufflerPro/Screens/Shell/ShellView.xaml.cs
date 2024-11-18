using System.Windows;
using System.Windows.Media;
using ShufflerPro.Framework.WPF.Controls;

namespace ShufflerPro.Screens.Shell;

public partial class ShellView
{
    private readonly SolidColorBrush? _darkBrush;
    private readonly SolidColorBrush? _lightBrush;

    public ShellView()
    {
        InitializeComponent();
        
        _darkBrush = FindResource("MahApps.Brushes.DataGrid.Selection.Background") as SolidColorBrush;
        _lightBrush = FindResource("MahApps.Brushes.ThemeForeground") as SolidColorBrush;
    }

    private void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is ClickSelectTextBox textBox)
        {
            textBox.Focus();
            textBox.SelectAll();
        }
    }

    private void UIElement_OnIsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        TitleTextBlock.Foreground = (bool)e.NewValue ? _darkBrush : _lightBrush;
        ArtistTextBlock.Foreground = (bool)e.NewValue ? _darkBrush : _lightBrush;
        AlbumTextBlock.Foreground = (bool)e.NewValue ? _darkBrush : _lightBrush;
    }
}