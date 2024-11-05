using LiteDB;

namespace ShufflerPro.Client.Entities;

public class Source(string folderPath, ObjectId id)
{
    public string FolderPath { get; } = folderPath;
    public ObjectId Id { get; } = id;
}