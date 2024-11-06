using ShufflerPro.Database;

namespace ShufflerPro.Client.Entities;

public class Playlist(string name)
{
    public LocalDatabaseKey? Id { get; private set; }
    public string Name { get; set; } = name;
    public int SongCount => Indexes.Count;

    public List<PlaylistIndex> Indexes { get; set; } = [];

    public void SetId(LocalDatabaseKey localDatabaseKey)
    {
        Id = localDatabaseKey;
    }
}