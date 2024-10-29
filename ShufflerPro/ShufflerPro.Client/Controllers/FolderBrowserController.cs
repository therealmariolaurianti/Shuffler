using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class FolderBrowserController
{
    public NewResult<ObservableCollection<SourceFolder>> BuildSourceFolders(string folderPath,
        ICollection<SourceFolder> existingSourceFolders)
    {
        var fullPath = Path.GetFullPath(folderPath);
        var root = Path.GetPathRoot((string?)folderPath)?.Replace(Path.DirectorySeparatorChar, ' ').Trim();
        if (root is null)
            return new Exception("Failed to load source");

        var rootFolder = existingSourceFolders.SingleOrDefault(sf => sf.IsRoot && sf.Header == root);
        if (rootFolder == null)
        {
            rootFolder = new SourceFolder(root, root, true, null);
            existingSourceFolders.Add(rootFolder);
        }

        var allFolders = existingSourceFolders.SelectMany(s => s.Items).ToList();
        allFolders.Add(rootFolder);

        var levels = fullPath.Split(Path.DirectorySeparatorChar).ToList();

        NewMethod(root, levels, rootFolder, allFolders, fullPath);

        return existingSourceFolders.ToObservableCollection();
    }

    private static void NewMethod(string root, List<string> levels, SourceFolder rootFolder,
        List<SourceFolder> allFolders, string fullPath)
    {
        foreach (var level in levels)
        {
            var index = levels.IndexOf(level);
            var currentPath = string.Join(Path.DirectorySeparatorChar, levels.Take(index + 1));

            if (level == root || allFolders.Any(af => af.Header == level && af.FullPath == currentPath))
                continue;

            var item = new SourceFolder(level, rootFolder, fullPath);

            var existingFolder = allFolders.SingleOrDefault(f => f.Header == item.Header
                                                                 && f.Parent?.Header == item.Parent?.Header);
            if (existingFolder is not null)
            {
                rootFolder = existingFolder;
            }
            else
            {
                rootFolder.Items.Add(item);
                rootFolder = item;
                allFolders.Add(item);
            }

            if (levels.Last() == level)
                try
                {
                    var directories = Directory.GetDirectories(fullPath);
                    if (!directories.Any())
                        continue;
                    foreach (var directory in directories)
                    {
                        var x = directory.Split(Path.DirectorySeparatorChar).ToList();
                        NewMethod(root, x, rootFolder, allFolders, directory);
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
        }
    }
}