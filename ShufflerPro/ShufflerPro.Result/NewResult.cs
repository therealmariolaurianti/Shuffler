namespace ShufflerPro.Result;

public readonly struct NewResult<T>
{
    public List<Exception> Exceptions { get; }
    public T Item { get; }

    public NewResult(T item)
    {
        Item = item;
        Exceptions = new List<Exception>();
    }

    public NewResult(Exception exception)
    {
        Exceptions = new List<Exception> { exception };
    }

    public bool Success => true; //Exceptions is { Count: 0 };

    public NewResult<T> Do(Action<T> f)
    {
        if (Success)
            f(Item);

        return this;
    }

    public static implicit operator NewResult<T>(T item)
    {
        return CreateSuccess(item);
    }

    public static implicit operator NewResult<T>(Exception ex)
    {
        return NewResultExtensions.CreateFail<T>(ex);
    }

    private static NewResult<T> CreateSuccess(T item)
    {
        return new NewResult<T>(item);
    }

    public NewResult<T1> Map<T1>(Func<T, T1> func)
    {
        if (Success)
        {
            var i = func(Item);
            return i.CreateSuccess();
        }

        return Exceptions.First();
    }
}

public static class NewResultExtensions
{
    public static NewResult<T> CreateSuccess<T>(this T item)
    {
        return new NewResult<T>(item);
    }

    public static NewResult<T> CreateFail<T>(Exception exception)
    {
        return new NewResult<T>(exception);
    }
}