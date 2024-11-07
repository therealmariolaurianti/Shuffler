using System.Windows;
using System.Windows.Input;
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

    private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        throw new NotImplementedException();
    }
}