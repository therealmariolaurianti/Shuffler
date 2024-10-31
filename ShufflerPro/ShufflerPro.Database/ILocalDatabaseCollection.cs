using ShufflerPro.Result;

namespace ShufflerPro.Database;

public interface ILocalDatabaseCollection<T>
{
    T FindById(LocalDatabaseKey i);
    NewResult<T> TryFindById(LocalDatabaseKey i);
    LocalDatabaseKey Insert(T item);
    bool Upsert(T item);
    bool Update(T item);
    IEnumerable<T> FindAll();
    bool Upsert(LocalDatabaseKey key, T item);
    LocalDatabaseQuery<T> Query();
}