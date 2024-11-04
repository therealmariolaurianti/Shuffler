using File = TagLib.File;

namespace ShufflerPro.Client.Entities;

public class Song(File? songFile, string path) : EntityBase
{
    private bool _isPlaying;
    public string? Genre { get; } = songFile?.Tag.FirstGenre;
    public string? Title { get; } = songFile?.Tag.Title;
    public int? Track { get; } = (int?)songFile?.Tag.Track;
    public string Artist { get; private set; } = songFile?.Tag.FirstAlbumArtist ?? "Unknown Artist";
    public string Album { get; } = songFile?.Tag.Album ?? "Unknown Album";
    public string? Path { get; set; } = path;
    public string? Time { get; } = songFile?.Properties.Duration.ToString("mm':'ss");
    public TimeSpan? Duration { get; } = songFile?.Properties.Duration;
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