namespace ShufflerPro.Client.Extensions;

public static class ExceptionExtensions
{
    public static IEnumerable<string> Messages(this Exception? ex)
    {
        while (ex != null)
        {
            yield return ex.Message;
            ex = ex.InnerException;
        }
    }
}