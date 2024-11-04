using LiteDB.Async;

namespace ShufflerPro.Database;

public class LocalDatabaseConnection : IDisposable
{
    private readonly LiteDatabaseAsync _liteDatabase;

    public LocalDatabaseConnection(LiteDatabaseAsync liteDatabase)
    {
        _liteDatabase = liteDatabase;
    }

    public void Dispose()
    {
        _liteDatabase.Dispose();
    }

    public LocalDatabaseCollection<T> GetCollection<T>(string? collectionName = null)
    {
        collectionName ??= typeof(T).Name;
        var collection = _liteDatabase.GetCollection<T>(collectionName);
        return new LocalDatabaseCollection<T>(collection);
    }
}