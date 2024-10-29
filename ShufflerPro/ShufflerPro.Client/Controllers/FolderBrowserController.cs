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
            rootFolder = new SourceFolder(root, root, true);
            existingSourceFolders.Add(rootFolder);
        }

        var allFolders = existingSourceFolders.SelectMany(s => s.Items).ToList();

        var levels = fullPath.Split(Path.DirectorySeparatorChar).ToList();
        foreach (var level in levels)
        {
            if (level == rootFolder.Header)
                continue;

            var item = new SourceFolder(level);
            
            rootFolder.Items.Add(item);
            rootFolder = item;
        }

        return existingSourceFolders.ToObservableCollection();
    }
}