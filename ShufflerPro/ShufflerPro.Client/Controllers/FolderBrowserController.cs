using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class FolderBrowserController
{
    public NewResult<ObservableCollection<SourceFolder>> BuildSourceFolders(string folderPath,
        ICollection<SourceFolder> existingSourceFolders)
    {
        var result = FindRoot(folderPath, existingSourceFolders)
            .Map(rootFolder =>
            {
                var allFolders = existingSourceFolders.SelectMany(s => s.Items).ToList();
                var levels = folderPath.Split(Path.DirectorySeparatorChar).ToList();

                Build(rootFolder.Header, levels, rootFolder, allFolders, folderPath);

                return existingSourceFolders.ToObservableCollection();
            });
        return result;
    }

    private static void Build(string root, List<string> levels, SourceFolder rootFolder,
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
                        Build(root, x, rootFolder, allFolders, directory);
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
        }
    }
    
    private NewResult<SourceFolder> FindRoot(string folderPath, ICollection<SourceFolder> existingSourceFolders)
    {
        var rootPath = Path.GetPathRoot((string?)folderPath)?.Replace(Path.DirectorySeparatorChar, ' ').Trim();
        if (rootPath is null)
            return new Exception("Failed to load source");

        var root = existingSourceFolders.SingleOrDefault(sf => sf.IsRoot && sf.Header == rootPath);
        return root ?? BuildRoot(rootPath, existingSourceFolders);
    }

    private NewResult<SourceFolder> BuildRoot(string rootPath, ICollection<SourceFolder> existingSourceFolders)
    {
        var root = new SourceFolder(rootPath, rootPath, true, null);
        existingSourceFolders.Add(root);
        return root;
    }
}