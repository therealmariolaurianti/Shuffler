using Caliburn.Micro;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Screens.Shell;

public class PlaylistGridItem : PropertyChangedBase
{
    private bool _isInEditMode;

    public PlaylistGridItem(Playlist playlist)
    {
        Item = playlist;
    }

    public bool IsInEditMode
    {
        get => _isInEditMode;
        set
        {
            if (value == _isInEditMode) return;
            _isInEditMode = value;
            NotifyOfPropertyChange();
        }
    }

    public string Name
    {
        get => Item.Name;
        set
        {
            if (value == Item.Name) return;
            Item.Name = value;
            NotifyOfPropertyChange();
        }
    }

    public Playlist Item { get; set; }
}