using ShufflerPro.Framework.WPF.Windows;

namespace ShufflerPro.Screens.Exceptions;

public partial class ExceptionView
{
    public ExceptionView()
    {
        InitializeComponent();
        WindowHelper.WindowsRoundCorners(GetWindow(this)!);
    }
}