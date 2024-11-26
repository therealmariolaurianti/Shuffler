using ShufflerPro.Framework.WPF.Windows;

namespace ShufflerPro.Screens.Setting;

public partial class SettingsView
{
    public SettingsView()
    {
        InitializeComponent();
        WindowHelper.WindowsRoundCorners(GetWindow(this)!);
    }
}