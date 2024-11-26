using ShufflerPro.Framework.WPF.Windows;

namespace ShufflerPro.Screens.ExcludedSongs;

public partial class ExcludedSongsView
{
    public ExcludedSongsView()
    {
        InitializeComponent();
        WindowHelper.WindowsRoundCorners(GetWindow(this));
    }
}