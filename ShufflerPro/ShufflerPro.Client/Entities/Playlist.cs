using LiteDB;

namespace ShufflerPro.Client.Entities;

public class Playlist(ObjectId? id, string name)
{
    public ObjectId? Id { get; private set; } = id;
    public string Name { get; set; } = name;
    public int SongCount => Indexes.Count;

    public List<PlaylistIndex> Indexes { get; set; } = [];

    public void SetId(ObjectId localDatabaseKey)
    {
        Id = localDatabaseKey;
    }
}