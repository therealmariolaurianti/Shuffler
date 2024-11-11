using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using ShufflerPro.Client;
using ShufflerPro.Client.Attributes;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.States;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;

namespace ShufflerPro.Screens.EditSong.Multiple;

public class EditSongsViewModel : ViewModelBase
{
    private readonly ItemTracker<EditSongsViewModel> _itemTracker;
    private readonly SongController _songController;
    private readonly List<Song> _songs;
    private string _album;
    private BitmapImage? _albumArt;
    private string _artist;
    private bool _canSave;
    private string _duration;
    private string? _genre;
    private bool _saving;
    private string? _title;
    private int? _track;

    public EditSongsViewModel(
        List<Song> songs,
        SongController songController,
        ItemTracker<EditSongsViewModel> itemTracker)
    {
        _songs = songs;
        _songController = songController;
        _itemTracker = itemTracker;

        Start();
        itemTracker.Attach(this);
    }

    public string? Genre
    {
        get => _genre;
        set
        {
            if (value == _genre) return;
            _genre = value;
            NotifyOfPropertyChange();
        }
    }

    public string Album
    {
        get => _album;
        set
        {
            if (value == _album) return;
            _album = value;
            NotifyOfPropertyChange();
        }
    }

    public string Artist
    {
        get => _artist;
        set
        {
            if (value == _artist) return;
            _artist = value;
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
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            NotifyOfPropertyChange();
        }
    }

    [IgnoreTracking]
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

    public int? Track
    {
        get => _track;
        set
        {
            if (value == _track) return;
            _track = value;
            NotifyOfPropertyChange();
        }
    }

    [IgnoreTracking]
    public string Duration
    {
        get => _duration;
        set
        {
            if (value == _duration) return;
            _duration = value;
            NotifyOfPropertyChange();
        }
    }

    private void Start()
    {
        var albums = _songs.Select(s => s.CreatedAlbum).Distinct().ToList();
        var artists = albums.Select(s => s.Artist).Distinct().ToList();

        SetSongTitle();
        SetAlbum(albums);
        SetArtist(artists);
        SetGenre();
        SetTrack();
        SetDuration();
    }

    private void SetArtist(List<Artist> artists)
    {
        Artist = artists.Count == 1 ? artists.First().Name : string.Empty;
    }

    private void SetAlbum(List<Album?> albums)
    {
        Album = albums.Count == 1 ? albums.First()?.Name ?? string.Empty : string.Empty;
    }

    private void SetSongTitle()
    {
        var songTitles = _songs.Select(s => s.Title).Distinct().ToList();
        Title = songTitles.Count == 1 ? songTitles.First() : string.Empty;
    }

    private void SetGenre()
    {
        var genres = _songs.Select(s => s.Genre).Distinct().ToList();
        Genre = genres.Count == 1 ? genres.First() : string.Empty;
    }

    private void SetDuration()
    {
        var durations = _songs.Select(s => s.Duration).Distinct().ToList();
        Duration = durations.Count == 1 ? durations.First()?.ToString("mm':'ss") ?? string.Empty : string.Empty;
    }

    private void SetTrack()
    {
        var tracks = _songs.Select(s => s.Track).Distinct().ToList();
        Track = tracks.Count == 1 ? tracks.First() : null;
    }

    public void Cancel()
    {
        _itemTracker.Revert();
        TryCloseAsync(false);
    }

    public override void NotifyOfPropertyChange([CallerMemberName] string? propertyName = null)
    {
        CanSave = _itemTracker.IsDirty; //|| _albumArtChanged;
        base.NotifyOfPropertyChange(propertyName);
    }

    public void Save()
    {
        _saving = true;
        RunAsync(async () => await _songController
            .Update(new UpdateSongsState(_songs, _itemTracker.PropertyDifferences))
            .IfFail(_ => MessageBox.Show("Failed to update song."))
            .IfSuccessAsync(async _ => await TryCloseAsync(true)));
    }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = new())
    {
        if (_itemTracker.IsDirty && !_saving)
            _itemTracker.Revert();
        return base.CanCloseAsync(cancellationToken);
    }
}