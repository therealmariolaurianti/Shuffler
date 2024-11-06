using System.Security.Cryptography;
using System.Text;
using File = TagLib.File;

namespace ShufflerPro.Client.Entities;

public class Song : EntityBase
{
    private bool _isPlaying;

    public Song(File? songFile, string path)
    {
        Genre = songFile?.Tag.FirstGenre;
        Title = songFile?.Tag.Title;
        Track = (int?)songFile?.Tag.Track;
        Artist = songFile?.Tag.FirstAlbumArtist ?? "Unknown Artist";
        Album = songFile?.Tag.Album ?? "Unknown Album";
        Path = path;
        Time = songFile?.Properties.Duration.ToString("mm':'ss");
        Duration = songFile?.Properties.Duration;

        var input = $"{Title}{Artist}{Album}{Time}";
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var result = new Guid(hash);
            
            Id = result;
        }
    }

    public Guid Id { get; }
    public string? Genre { get; }
    public string? Title { get; }
    public int? Track { get; }
    public string Artist { get; private set; }
    public string Album { get; }
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