using TagLib;

namespace ShufflerPro.Upgraded.Objects;

public class SongFactory
{
    public static Song Create(string songPath)
    {
        try
        {
            var songFile = File.Create(songPath);
            return new Song(songFile, songPath);
        }
        catch (Exception)
        {
            return new Song(null, songPath);
        }
    }
}

public struct Song(File? songFile, string path)
{
    public string? Genre { get; } = songFile?.Tag.FirstGenre;
    public string? Title { get; } = songFile?.Tag.Title;
    public int? Track { get; } = (int?)songFile?.Tag.Track;
    public string Artist { get; private set; } = songFile?.Tag.FirstAlbumArtist ?? "Unknown Artist";
    public string Album { get; } = songFile?.Tag.Album ?? "Unknown Album";
    public string? Path { get; set; } = path;
    public string? Time { get; } = songFile?.Properties.Duration.ToString("mm':'ss");

    private bool Equals(Song other)
    {
        return string.Equals(Title, other.Title);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
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
}