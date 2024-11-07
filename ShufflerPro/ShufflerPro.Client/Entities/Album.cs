namespace ShufflerPro.Client.Entities;

public class Album
{
    public Album(Artist artist, string name, List<Song> songs)
    {
        Artist = artist;
        Name = name;

        AddSongs(songs);
    }

    public string Name { get; }
    public List<Song> Songs { get; private set; } = [];
    public Artist Artist { get; set; }

    private void AddSongs(List<Song> songs)
    {
        var list = songs.OrderBy(s => s.Track).ToList();
        songs.ForEach(s => s.CreatedAlbum = this);
        Songs = list;
    }

    protected bool Equals(Album other)
    {
        return Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Album)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}