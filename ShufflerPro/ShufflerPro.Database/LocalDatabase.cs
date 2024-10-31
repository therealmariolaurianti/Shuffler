using LiteDB;

namespace ShufflerPro.Database;

public class LocalDatabase
{
    public LocalDatabaseConnection CreateConnection(string fileName)
    {
        return new LocalDatabaseConnection(CreateDatabase(fileName));
    }

    private LiteDatabase CreateDatabase(string fileName)
    {
        var directory = Path.GetDirectoryName(fileName);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        var liteDatabase = new LiteDatabase(fileName);
        return liteDatabase;
    }
}