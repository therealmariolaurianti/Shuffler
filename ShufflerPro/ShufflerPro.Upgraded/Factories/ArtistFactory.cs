using ShufflerPro.Upgraded.Objects;

namespace ShufflerPro.Upgraded.Factories;

public class ArtistFactory
{
    public Artist Create(string name, List<Album> albums)
    {
        return new Artist(name, albums);
    }
}