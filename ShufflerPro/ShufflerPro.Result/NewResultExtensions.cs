namespace ShufflerPro.Result;

public static class NewResultExtensions
{
    public static NewResult<T> CreateSuccess<T>(this T item)
    {
        return new NewResult<T>(item);
    }

    public static NewResult<T> CreateFail<T>(Exception ex)
    {
        return new NewResult<T>(ex);
    }

    public static NewResult<T> Try<T>(Func<T> func)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static async Task<NewResult<T1>> Try<T1>(Func<Task<T1>> func)
    {
        try
        {
            var newResult = await func();
            return newResult;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static async Task<NewResult<T>> Do<T>(this Task<NewResult<T>> ma, Action<T> f)
    {
        var item = await ma;
        item.IfSuccess(f);
        return item;
    }
}