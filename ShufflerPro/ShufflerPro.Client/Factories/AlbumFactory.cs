using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class AlbumFactory
{
    public Album Create(Artist artist, string name, List<Song> songs)
    {
        return new Album(artist, name, songs);
    }
}