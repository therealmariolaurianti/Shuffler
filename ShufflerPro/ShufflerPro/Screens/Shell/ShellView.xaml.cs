using System.Windows;
using ShufflerPro.Framework.WPF.Controls;
using ShufflerPro.Framework.WPF.Windows;

namespace ShufflerPro.Screens.Shell;

public partial class ShellView
{
    public ShellView()
    {
        InitializeComponent();
        WindowHelper.WindowsRoundCorners(GetWindow(this)!);
    }

    private void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is ClickSelectTextBox textBox)
        {
            textBox.Focus();
            textBox.SelectAll();
        }
    }
}