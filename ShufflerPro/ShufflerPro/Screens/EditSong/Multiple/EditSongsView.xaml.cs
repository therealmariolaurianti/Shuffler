using ShufflerPro.Framework.WPF.Windows;

namespace ShufflerPro.Screens.EditSong.Multiple;

public partial class EditSongsView
{
    public EditSongsView()
    {
        InitializeComponent();
        WindowHelper.WindowsRoundCorners(GetWindow(this));
    }
}