namespace ShufflerPro.Client.Entities;

public class Artist
{
    public Artist(string name, List<Album> albums)
    {
        Name = name;
        Albums = albums.OrderBy(a => a.Name).ToList();
    }

    public string Name { get; }
    public List<Album> Albums { get; }
}