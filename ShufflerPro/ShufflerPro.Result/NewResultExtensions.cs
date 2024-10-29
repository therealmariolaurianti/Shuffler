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
}