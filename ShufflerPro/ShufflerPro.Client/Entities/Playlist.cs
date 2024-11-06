using LiteDB;

namespace ShufflerPro.Client.Entities;

public class Playlist(string name)
{
    public ObjectId? Id { get; private set; }
    public string Name { get; set; } = name;
    public int SongCount => Indexes.Count;

    public List<PlaylistIndex> Indexes { get; set; } = [];

    public void SetId(ObjectId localDatabaseKey)
    {
        Id = localDatabaseKey;
    }
}