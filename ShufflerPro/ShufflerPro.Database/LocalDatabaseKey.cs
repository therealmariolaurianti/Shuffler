using LiteDB;

namespace ShufflerPro.Database;

public class LocalDatabaseKey
{
    private readonly BsonValue _value;

    public LocalDatabaseKey(BsonValue value)
    {
        _value = value;
    }

    public object Value => _value;
}