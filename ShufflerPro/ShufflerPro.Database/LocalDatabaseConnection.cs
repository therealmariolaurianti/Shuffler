using LiteDB;

namespace ShufflerPro.Database;

public class LocalDatabaseConnection : IDisposable
{
    private readonly LiteDatabase _liteDatabase;

    public LocalDatabaseConnection(LiteDatabase liteDatabase)
    {
        _liteDatabase = liteDatabase;
    }

    public void Dispose()
    {
        _liteDatabase?.Dispose();
    }

    public ILocalDatabaseCollection<T> GetCollection<T>(string? collectionName = null)
    {
        collectionName ??= typeof(T).Name;
        return new LocalDatabaseCollection<T>(_liteDatabase.GetCollection<T>(collectionName));
    }
}