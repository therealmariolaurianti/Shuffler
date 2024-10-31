using LiteDB.Async;

namespace ShufflerPro.Database;

public class LocalDatabaseCollection<T>
{
    private readonly ILiteCollectionAsync<T> _collection;

    public LocalDatabaseCollection(ILiteCollectionAsync<T> collection)
    {
        _collection = collection;
    }

    public async Task<T> FindById(LocalDatabaseKey id)
    {
        var bsonValue = id.AsBsonValue();
        return await _collection.FindByIdAsync(bsonValue);
    }

    public async Task<LocalDatabaseKey> Insert(T item)
    {
        var value = await _collection.InsertAsync(item);
        return new LocalDatabaseKey(value);
    }

    public async Task<bool> Upsert(T item)
    {
        return await _collection.UpsertAsync(item);
    }

    public async Task<bool> Update(T item)
    {
        return await _collection.UpdateAsync(item);
    }

    public async Task<IEnumerable<T>> FindAll()
    {
        return await _collection.FindAllAsync();
    }

    public async Task<bool> Upsert(LocalDatabaseKey key, T item)
    {
        return await _collection.UpsertAsync(key.AsBsonValue(), item);
    }
}