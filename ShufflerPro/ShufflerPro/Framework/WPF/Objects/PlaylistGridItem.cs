using Caliburn.Micro;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Framework.WPF.Objects;

public class PlaylistGridItem(Playlist playlist) : PropertyChangedBase
{
    private bool _isInEditMode;

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

    public Playlist Item { get; } = playlist;
}