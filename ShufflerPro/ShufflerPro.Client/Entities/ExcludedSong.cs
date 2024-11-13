using LiteDB;

namespace ShufflerPro.Client.Entities;

public class ExcludedSong
{
    public ExcludedSong(ObjectId id, Guid songId)
    {
        Id = id;
        SongId = songId;
    }

    public ObjectId Id { get; set; }
    public Guid SongId { get; set; }
}