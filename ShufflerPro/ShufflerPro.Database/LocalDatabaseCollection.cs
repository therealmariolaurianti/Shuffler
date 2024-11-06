using System.Linq.Expressions;
using LiteDB;
using LiteDB.Async;

namespace ShufflerPro.Database;

public static class Extensions
{
    public static BsonValue AsBsonValue(this LocalDatabaseKey key)
    {
        return new BsonValue(key.Value);
    }
}

public class LocalDatabaseCollection<T>
{
    private readonly ILiteCollectionAsync<T> _collection;

    public LocalDatabaseCollection(ILiteCollectionAsync<T> collection)
    {
        _collection = collection;
    }
    
    public async Task<LocalDatabaseKey> Insert(T item)
    {
        var value = await _collection.InsertAsync(item);
        return new LocalDatabaseKey(value);
    }

    public async Task<bool> Delete(LocalDatabaseKey id)
    {
        var bsonValue = id.AsBsonValue();
        return await _collection.DeleteAsync(bsonValue);
    }
    
    public async Task<bool> Update(T item)
    {
        return await _collection.UpdateAsync(item);
    }

    public async Task<int> DeleteAll()
    {
        return await _collection.DeleteAllAsync();
    }

    public async Task<IEnumerable<T>> FindAll()
    {
        return await _collection.FindAllAsync();
    }
}