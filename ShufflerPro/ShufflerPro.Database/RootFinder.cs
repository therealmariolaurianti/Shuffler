using ShufflerPro.Result;

namespace ShufflerPro.Database;

public static class RootFinder
{
    public static NewResult<string> FindRoot(string? directory = null)
    {
        var currentDirectory = directory ?? Directory.GetCurrentDirectory();
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