using LiteDB;
using ShufflerPro.Result;

namespace ShufflerPro.Database;

public class LocalDatabaseCollection<T> : ILocalDatabaseCollection<T>
{
    private readonly ILiteCollection<T> _collection;

    public LocalDatabaseCollection(ILiteCollection<T> collection)
    {
        _collection = collection;
    }

    public T FindById(LocalDatabaseKey id)
    {
        var bsonValue = id.AsBsonValue();
        return _collection.FindById(bsonValue);
    }

    public NewResult<T> TryFindById(LocalDatabaseKey id)
    {
        return NewResultExtensions.Try(() =>
        {
            var bsonValue = id.AsBsonValue();
            return _collection.FindById(bsonValue);
        });
    }


    public LocalDatabaseKey Insert(T item)
    {
        var value = _collection.Insert(item);
        return new LocalDatabaseKey(value);
    }

    public bool Upsert(T item)
    {
        return _collection.Upsert(item);
    }

    public bool Update(T item)
    {
        return _collection.Update(item);
    }

    public IEnumerable<T> FindAll()
    {
        return _collection.FindAll();
    }

    public bool Upsert(LocalDatabaseKey key, T item)
    {
        return _collection.Upsert(key.AsBsonValue(), item);
    }

    public LocalDatabaseQuery<T> Query()
    {
        return new LocalDatabaseQuery<T>(_collection.Query());
    }
}