using ShufflerPro.Database.Interfaces;

namespace ShufflerPro.Database;

public class DatabasePath : IDatabasePath
{
    public DatabasePath()
    {
        var root = FindRoot();
        Path = $@"{root}\local.db";
    }

    public string Path { get; }

    public static string FindRoot()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        while (true)
        {
            if (Directory.GetFiles(currentDirectory, ".root").Any())
                return currentDirectory;
            var directoryInfo = Directory.GetParent(currentDirectory);
            if (directoryInfo == null)
                throw new ApplicationException("Cannot find .root");
            currentDirectory = directoryInfo.FullName;
        }
    }
}