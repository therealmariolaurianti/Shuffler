using LiteDB.Async;

namespace ShufflerPro.Database;

public class LocalDatabase
{
    public LocalDatabaseConnection CreateConnection(string fileName)
    {
        return new LocalDatabaseConnection(CreateDatabase(fileName));
    }

    private LiteDatabaseAsync CreateDatabase(string fileName)
    {
        var liteDatabase = new LiteDatabaseAsync(fileName);
        return liteDatabase;
    }
}