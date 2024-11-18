using System.Windows;
using ShufflerPro.Framework.WPF.Controls;

namespace ShufflerPro.Screens.Shell;

public partial class ShellView
{
    public ShellView()
    {
        InitializeComponent();
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