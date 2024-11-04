using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class ArtistFactory
{
    public Artist Create(string name, List<Album> albums)
    {
        return new Artist(name, albums);
    }
}