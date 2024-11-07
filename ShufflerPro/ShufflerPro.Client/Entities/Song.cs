using ShufflerPro.Client.Extensions;
using File = TagLib.File;

namespace ShufflerPro.Client.Entities;

public class Song : EntityBase
{
    private bool _isPlaying;

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
    public string? Genre { get; set; }
    public string? Title { get; set; }
    public int? Track { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }
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