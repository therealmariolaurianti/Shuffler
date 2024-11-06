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

    public static NewResult<T> CreateFail<T>(string message)
    {
        return new NewResult<T>(new Exception(message));
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

    public static async Task<NewResult<T1>> Bind<T, T1>(this Task<NewResult<T>> ma,
        Func<T, NewResult<T1>> func)
    {
        try
        {
            var result = await ma;
            return result.Bind(func);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static async Task<NewResult<T1>> Bind<T, T1>(this Task<NewResult<T>> ma,
        Func<T, Task<NewResult<T1>>> func)
    {
        try
        {
            var result = await ma;
            return await result.Bind(func);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static async Task<NewResult<T>> IfFail<T>(this Task<NewResult<T>> ma, Action<Exception> f)
    {
        var item = await ma;
        return item.IfFail(ex => f(ex));
    }

    public static async Task<NewResult<T>> IfSuccess<T>(this Task<NewResult<T>> task, Action<T> f)
    {
        var item = await task;
        item.IfSuccess(f);
        return item;
    }

    public static async Task<NewResult<NewUnit>> ToSuccessAsync<T>(this Task<NewResult<T>> item)
    {
        return await NewUnit.DefaultAsync;
    }
}