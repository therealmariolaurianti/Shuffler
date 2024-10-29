namespace ShufflerPro.Result;

public class NewResult<T>
{
    public List<Exception>? Exceptions { get; }
    public T Item { get; }

    public bool Success => Exceptions != null && Exceptions.Count == 0;

    public NewResult<T> Do(Action<T> f)
    {
        if (Success)
            f(Item);
        return this;
    }
}