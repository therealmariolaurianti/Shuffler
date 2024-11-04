namespace ShufflerPro.Result;

public class NewResult<T>
{
    public NewResult(T item)
    {
        Item = item;
        Exceptions = new List<Exception>();
    }

    public NewResult()
    {
        Item = default;
        Exceptions = new List<Exception>();
    }

    public NewResult(Exception exception)
    {
        Exceptions = new List<Exception> { exception };
    }

    private NewResult(IEnumerable<Exception> exceptions)
    {
        Exceptions = exceptions.ToList();
    }

    public List<Exception> Exceptions { get; }
    public T Item { get; }

    public bool Success => Exceptions is { Count: 0 };

    public bool Fail => !Success;

    private Exception FirstException => Exceptions.First();

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

    public NewResult<T> IfSuccess(Action<T> action)
    {
        if (Success)
            action(Item);
        return this;
    }

    public NewResult<T1> Bind<T1>(Func<T, NewResult<T1>> func)
    {
        return !Success ? NewResultExtensions.CreateFail<T1>(FirstException) : func(Item);
    }

    public async Task<NewResult<T1>> Bind<T1>(Func<T, Task<NewResult<T1>>> func)
    {
        return !Success ? NewResultExtensions.CreateFail<T1>(FirstException) : await func(Item);
    }
    
    public NewResult<T> IfFail(Action<Exception> func)
    {
        if (!Success)
            func(FirstException);
        return this;
    }
    
    public async Task IfSuccessAsync(Func<T, Task> action)
    {
        if (Success)
            await action(Item);
    }
}