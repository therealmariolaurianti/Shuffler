using ShufflerPro.Database.Interfaces;
using ShufflerPro.Result;

namespace ShufflerPro.Database;

public class DatabasePath : IDatabasePath
{
    private readonly string _currentDirectory;

    public DatabasePath()
    {
        _currentDirectory = Directory.GetCurrentDirectory();
    }

    public DatabasePath(string currentDirectory)
    {
        _currentDirectory = currentDirectory;
    }

    public string Path { get; private set; } = string.Empty;

    public NewResult<string> Start()
    {
        return RootFinder.FindRoot(_currentDirectory)
            .IfSuccess(root => Path = $@"{root}\local.db");
    }
}