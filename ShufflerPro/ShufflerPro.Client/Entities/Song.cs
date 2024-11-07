using ShufflerPro.Client.Extensions;
using File = TagLib.File;

namespace ShufflerPro.Client.Entities;

public class Song : EntityBase
{
    private string _album;
    private string _artist;
    private string? _genre;
    private bool _isPlaying;
    private string? _title;
    private int? _track;

    public Song(File? songFile, string path)
    {
        Id = this.Hash(path);
        Path = path;

        Genre = songFile?.Tag.Genres?.ToFormattedString(",");
        Artist = songFile?.Tag.FirstAlbumArtist ?? songFile?.Tag.Performers?.ToFormattedString(",") ?? "Unknown Artist";
        Title = songFile?.Tag.Title;
        Track = (int?)songFile?.Tag.Track;
        Album = songFile?.Tag.Album ?? "Unknown Album";
        Time = songFile?.Properties.Duration.ToString("mm':'ss");
        Duration = songFile?.Properties.Duration;
    }

    public Guid Id { get; }

    public string? Genre
    {
        get => _genre;
        set
        {
            if (value == _genre) return;
            _genre = value;
            OnPropertyChanged();
        }
    }

    public string? Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            OnPropertyChanged();
        }
    }

    public int? Track
    {
        get => _track;
        set
        {
            if (value == _track) return;
            _track = value;
            OnPropertyChanged();
        }
    }

    public string Artist
    {
        get => _artist;
        set
        {
            if (value == _artist) return;
            _artist = value;
            OnPropertyChanged();
        }
    }

    public string Album
    {
        get => _album;
        set
        {
            if (value == _album) return;
            _album = value;
            OnPropertyChanged();
        }
    }

    public string? Path { get; set; }
    public string? Time { get; }
    public TimeSpan? Duration { get; }
    public Album? CreatedAlbum { get; set; }

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (value == _isPlaying) return;
            _isPlaying = value;
            OnPropertyChanged();
        }
    }
}