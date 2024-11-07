using ShufflerPro.Client.Entities;
using ShufflerPro.Framework.WPF;

namespace ShufflerPro.Screens.EditSong;

public class EditSongViewModel : ViewModelBase
{
    private readonly Song _song;

    public EditSongViewModel(Song song)
    {
        _song = song;
    }
}