using System.Linq.Expressions;
using LiteDB.Async;

namespace ShufflerPro.Database;

public class LocalDatabaseQuery<T>
{
    private readonly ILiteQueryableAsync<T> _query;

    public LocalDatabaseQuery(ILiteQueryableAsync<T> query)
    {
        _query = query;
    }

    public LocalDatabaseQuery<T> Where(Expression<Func<T, bool>> func)
    {
        var liteQueryableAsync = _query.Where(func);
        return new LocalDatabaseQuery<T>(liteQueryableAsync);
    }

    public async Task<int> Count()
    {
        return await _query.CountAsync();
    }
}