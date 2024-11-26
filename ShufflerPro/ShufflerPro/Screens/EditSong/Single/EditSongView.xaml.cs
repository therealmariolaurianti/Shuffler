using ShufflerPro.Framework.WPF.Windows;

namespace ShufflerPro.Screens.EditSong.Single;

public partial class EditSongView
{
    public EditSongView()
    {
        InitializeComponent();
        WindowHelper.WindowsRoundCorners(GetWindow(this)!);
    }
}