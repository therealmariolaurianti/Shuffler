namespace ShufflerPro.Client.Entities;

public class Playlist(string name)
{
    public string Name { get; set; } = name;
    public Dictionary<Guid, int> Index { get; set; } = new();
    public int SongCount => Index.Count;
}