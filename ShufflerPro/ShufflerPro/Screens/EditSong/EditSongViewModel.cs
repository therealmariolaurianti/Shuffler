using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Framework;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;

namespace ShufflerPro.Screens.EditSong;

public class EditSongViewModel : ViewModelBase
{
    private readonly BinaryHelper _binaryHelper;
    private readonly ItemTracker<Song> _itemTracker;
    private readonly Song _song;
    private readonly SongController _songController;
    private bool _canSave;

    public EditSongViewModel(
        Song song,
        BitmapImage? albumArt,
        ItemTracker<Song> itemTracker,
        SongController songController, BinaryHelper binaryHelper)
    {
        _song = song;
        AlbumArt = albumArt;
        _itemTracker = itemTracker;
        _songController = songController;
        _binaryHelper = binaryHelper;

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

    public BitmapImage? AlbumArt { get; }

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
            .IfFail(_ => MessageBox.Show("Failed to update song."))
            .IfSuccessAsync(async _ => await TryCloseAsync(true)));
    }

    public void Cancel()
    {
        _itemTracker.Revert();
        TryCloseAsync(false);
    }

    public void ChangeAlbumArt()
    {
        _binaryHelper
            .Add()
            .IfSuccess(_ =>
            {
                
            });
    }
}