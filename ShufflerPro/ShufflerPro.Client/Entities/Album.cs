namespace ShufflerPro.Client.Entities;

public struct Album
{
    public Album(string artist, string name, List<Song> songs)
    {
        Artist = artist;
        Name = name;
        Songs = songs.OrderBy(s => s.Track).ToList();
    }

    public string Artist { get; }
    public string Name { get; }
    public List<Song> Songs { get; }
}