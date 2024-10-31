using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class ArtistFactory
{
    public Artist Create(string name, List<Album> albums)
    {
        var artist = new Artist(name, albums);
        albums.ForEach(a => a.CreatedArtist = artist);
        return artist;
    }
}