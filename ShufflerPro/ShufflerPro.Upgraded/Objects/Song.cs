using System.Diagnostics;
using TagLib;

namespace ShufflerPro.Upgraded.Objects;

[DebuggerDisplay("{Title}")]
public class Song
{
    public Song(File songFile, string path)
    {
        Path = path;
        Title = songFile.Tag.Title;
        Track = (int)songFile.Tag.Track;
        Artist = songFile.Tag.FirstAlbumArtist;
        Album = songFile.Tag.Album;
        Genre = songFile.Tag.FirstGenre;
        Id = NextId.GetNext();
    }

    public string Genre { get; }
    public int Id { get; }
    public string Title { get; }
    public int Track { get; }
    public string Artist { get; private set; }
    public string Album { get; }
    public string Path { get; set; }

    protected bool Equals(Song other)
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
        return Title.GetHashCode();
    }

    public void UpdateArtist(string artist)
    {
        Artist = artist;
    }
}