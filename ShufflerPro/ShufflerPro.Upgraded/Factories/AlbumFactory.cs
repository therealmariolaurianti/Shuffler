using ShufflerPro.Upgraded.Objects;

namespace ShufflerPro.Upgraded.Factories;

public class AlbumFactory
{
    public Album Create(string artist, string name, List<Song> songs)
    {
        return new Album(artist, name, songs);
    }
}