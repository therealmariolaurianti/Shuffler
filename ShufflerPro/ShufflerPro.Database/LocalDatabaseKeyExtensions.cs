using LiteDB;

namespace ShufflerPro.Database;

public static class LocalDatabaseKeyExtensions
{
    public static BsonValue AsBsonValue(this LocalDatabaseKey key)
    {
        return new BsonValue(key.Value);
    }
}