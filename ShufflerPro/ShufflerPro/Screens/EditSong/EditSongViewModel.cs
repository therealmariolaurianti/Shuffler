using System.Runtime.CompilerServices;
using System.Windows;
using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;

namespace ShufflerPro.Screens.EditSong;

public class EditSongViewModel : ViewModelBase
{
    private readonly ItemTracker<Song> _itemTracker;
    private readonly Song _song;
    private readonly SongController _songController;
    private bool _canSave;

    public EditSongViewModel(Song song, ItemTracker<Song> itemTracker, SongController songController)
    {
        _song = song;
        _itemTracker = itemTracker;
        _songController = songController;

        itemTracker.Attach(song);
    }

    public string? Path => _song.Path;
    public string Duration => _song.Duration?.ToString("mm':'ss") ?? string.Empty;

    public string? Genre
    {
        get => _song.Genre;
        set
        {
            if (value == _song.Genre) return;
            _song.Genre = value;
            NotifyOfPropertyChange();
        }
    }

    public int? Track
    {
        get => _song.Track;
        set
        {
            if (value == _song.Track) return;
            _song.Track = value;
            NotifyOfPropertyChange();
        }
    }

    public string Album
    {
        get => _song.Album;
        set
        {
            if (value == _song.Album) return;
            _song.Album = value;
            NotifyOfPropertyChange();
        }
    }

    public string Artist
    {
        get => _song.Artist;
        set
        {
            if (value == _song.Artist) return;
            _song.Artist = value;
            NotifyOfPropertyChange();
        }
    }

    public string? Title
    {
        get => _song.Title;
        set
        {
            if (value == _song.Title) return;
            _song.Title = value;
            NotifyOfPropertyChange();
        }
    }

    public bool CanSave
    {
        get => _canSave;
        set
        {
            if (value == _canSave) return;
            _canSave = value;
            NotifyOfPropertyChange();
        }
    }

    public override void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
    {
        CanSave = _itemTracker.IsDirty;
        base.NotifyOfPropertyChange(propertyName);
    }

    public void Save()
    {
        RunAsync(async () => await _songController
            .Update(_itemTracker)
            .IfFail(_ => MessageBox.Show("Failed to update song.")));
    }

    public void Cancel()
    {
        TryCloseAsync(false);
    }
}