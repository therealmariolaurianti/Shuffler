using ShufflerPro.Client.Entities;
using ShufflerPro.Framework.WPF;

namespace ShufflerPro.Screens.EditSong;

public class EditSongViewModel : ViewModelBase
{
    private string? _album;
    private string? _artist;
    private string? _genre;
    private string? _title;
    private int? _track;

    public EditSongViewModel(Song song)
    {
        Title = song.Title;
        Artist = song.Artist;
        Album = song.Album;
        Track = song.Track;
        Genre = song.Genre;

        Duration = song.Duration!.Value.ToString("mm':'ss");
        Path = song.Path;
    }

    public string? Path { get; }
    public string Duration { get; }

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

    public string? Album
    {
        get => _album;
        set
        {
            if (value == _album) return;
            _album = value;
            NotifyOfPropertyChange();
        }
    }

    public string? Artist
    {
        get => _artist;
        set
        {
            if (value == _artist) return;
            _artist = value;
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
}