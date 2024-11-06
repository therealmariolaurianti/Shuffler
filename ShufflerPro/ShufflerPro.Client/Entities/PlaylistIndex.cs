using LiteDB;

namespace ShufflerPro.Client.Entities;

public class PlaylistIndex(ObjectId id, Guid songId, int index, ObjectId? playlistId)
{
    public Guid SongId { get; } = songId;
    public int Index { get; } = index;
    public ObjectId? PlaylistId { get; } = playlistId;
    public ObjectId? Id { get; set; } = id;

    public void SetId(BsonValue asBsonValue)
    {
        Id = asBsonValue;
    }
}