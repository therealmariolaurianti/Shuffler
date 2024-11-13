using LiteDB;

namespace ShufflerPro.Client.Entities;

public class ExcludedSong
{
    public ExcludedSong(ObjectId id, Guid songId, string? songPath)
    {
        Id = id;
        SongId = songId;
        SongPath = songPath ?? string.Empty;
    }

    public ObjectId Id { get; set; }
    public Guid SongId { get; set; }
    public string SongPath { get; }
}