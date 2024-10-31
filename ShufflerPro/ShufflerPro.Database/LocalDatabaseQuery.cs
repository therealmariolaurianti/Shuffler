using System.Linq.Expressions;
using LiteDB;

namespace ShufflerPro.Database;

public class LocalDatabaseQuery<T>
{
    private readonly ILiteQueryable<T> _query;

    public LocalDatabaseQuery(ILiteQueryable<T> query)
    {
        _query = query;
    }

    public LocalDatabaseQuery<T> Where(Expression<Func<T, bool>> func)
    {
        return new LocalDatabaseQuery<T>(_query.Where(func));
    }

    public int Count()
    {
        return _query.Count();
    }
}