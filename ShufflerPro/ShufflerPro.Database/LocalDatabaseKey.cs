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

    public static implicit operator LocalDatabaseKey(int value)
    {
        return new LocalDatabaseKey(value);
    }

    public static implicit operator LocalDatabaseKey(Guid value)
    {
        return new LocalDatabaseKey(value);
    }

    public static implicit operator LocalDatabaseKey(string value)
    {
        return new LocalDatabaseKey(value);
    }
}