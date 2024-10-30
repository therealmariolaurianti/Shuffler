﻿namespace ShufflerPro.Result;

public class NewResult<T>
{
    public NewResult(T item)
    {
        Item = item;
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
}