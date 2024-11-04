using System.Linq.Expressions;
using LiteDB.Async;

namespace ShufflerPro.Database;

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

    public async Task<int> Delete(Expression<Func<T, bool>> predicate)
    {
        return await _collection.DeleteManyAsync(predicate);
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