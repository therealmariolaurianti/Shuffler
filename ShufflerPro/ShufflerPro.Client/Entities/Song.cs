using System.ComponentModel;
using System.Runtime.CompilerServices;
using TagLib;
using File = TagLib.File;

namespace ShufflerPro.Client.Entities;

public class Song(File? songFile, string path) : INotifyPropertyChanged
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
    public IPicture? Picture { get; } = songFile?.Tag.Pictures.FirstOrDefault();

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

    public Album? CreatedAlbum { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool Equals(Song other)
    {
        return string.Equals(Title, other.Title);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (obj.GetType() != GetType()) return false;
        return Equals((Song)obj);
    }

    public override int GetHashCode()
    {
        return Title?.GetHashCode() ?? 0;
    }

    public static bool operator ==(Song left, Song right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Song left, Song right)
    {
        return !(left == right);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}