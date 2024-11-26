using ShufflerPro.Framework.WPF.Windows;

namespace ShufflerPro.Screens.Startup;

public partial class StartupView
{
    public StartupView()
    {
        InitializeComponent();
        WindowHelper.WindowsRoundCorners(GetWindow(this));
    }
}