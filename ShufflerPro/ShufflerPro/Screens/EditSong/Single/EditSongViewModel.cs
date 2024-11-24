using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.States;
using ShufflerPro.Framework;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;

namespace ShufflerPro.Screens.EditSong.Single;

public class EditSongViewModel : ViewModelBase
{
    private readonly BinaryHelper _binaryHelper;
    private readonly ItemTracker<Song> _itemTracker;
    private readonly Library _library;
    private readonly Song _song;
    private readonly SongController _songController;

    private readonly ShufflerWindowManager _windowManager;
    private BitmapImage? _albumArt;
    private bool _albumArtChanged;
    private bool _canSave;
    private bool _saving;

    public EditSongViewModel(
        Song song,
        BitmapImage? albumArt,
        Library library,
        ItemTracker<Song> itemTracker,
        SongController songController,
        BinaryHelper binaryHelper, ShufflerWindowManager windowManager)
    {
        _song = song;
        _library = library;
        _itemTracker = itemTracker;
        _songController = songController;
        _binaryHelper = binaryHelper;
        _windowManager = windowManager;
        AlbumArt = albumArt;

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

    public BitmapImage? AlbumArt
    {
        get => _albumArt;
        set
        {
            if (Equals(value, _albumArt)) return;
            _albumArt = value;
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

    public override void NotifyOfPropertyChange([CallerMemberName] string? propertyName = null)
    {
        CanSave = _itemTracker.IsDirty || _albumArtChanged;
        base.NotifyOfPropertyChange(propertyName);
    }

    public void Save()
    {
        _saving = true;
        RunAsync(async () => await _songController
            .Update(new UpdateSongsState([_song], _itemTracker.PropertyDifferences,
                new AlbumArtState(_binaryHelper.ToBytes(AlbumArt), _albumArtChanged), _library))
            .IfFail(exception => _windowManager.ShowException(exception))
            .IfSuccessAsync(async _ => await TryCloseAsync(true)));
    }

    public void Cancel()
    {
        _itemTracker.Revert();
        TryCloseAsync(false);
    }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = new())
    {
        if (_itemTracker.IsDirty && !_saving)
            _itemTracker.Revert();
        return base.CanCloseAsync(cancellationToken);
    }

    public void ChangeAlbumArt()
    {
        _binaryHelper
            .Add()
            .IfSuccess(bytes =>
            {
                _albumArtChanged = true;
                AlbumArt = _binaryHelper.ToImage(bytes);
            });
    }
}