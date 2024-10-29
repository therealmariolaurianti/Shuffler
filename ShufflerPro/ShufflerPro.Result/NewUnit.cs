namespace ShufflerPro.Result;

public class NewUnit
{
    public static NewUnit Default { get; } = new NewUnit();
    public static Task<NewUnit> DefaultAsync => Task.FromResult(Default);
}