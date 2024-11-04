using ShufflerPro.Database.Interfaces;
using ShufflerPro.Result;

namespace ShufflerPro.Database;

public class DatabasePath : IDatabasePath
{
    private readonly string _currentDirectory;

    public DatabasePath(string currentDirectory)
    {
        _currentDirectory = currentDirectory;
    }

    public string Path { get; private set; } = string.Empty;

    public NewResult<string> Start()
    {
        return FindRoot()
            .IfSuccess(root => Path = $@"{root}\local.db");
    }

    public NewResult<string> FindRoot()
    {
        var currentDirectory = _currentDirectory;
        while (true)
        {
            if (Directory.GetFiles(currentDirectory, ".root").Any())
                return currentDirectory;
            var directoryInfo = Directory.GetParent(currentDirectory);
            if (directoryInfo == null)
                return NewResultExtensions.CreateFail<string>(new Exception("Failed to find root."));
            currentDirectory = directoryInfo.FullName;
        }
    }
}