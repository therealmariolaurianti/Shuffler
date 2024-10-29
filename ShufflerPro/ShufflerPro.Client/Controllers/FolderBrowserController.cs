using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class FolderBrowserController
{
    public NewResult<ObservableCollection<SourceFolder>> BuildSourceFolders(string folderPath,
        ICollection<SourceFolder> existingSourceFolders)
    {
        var folderName = Path.GetFileName((string?)folderPath);
        var fullPath = Path.GetFullPath(folderPath);
        var root = Path.GetPathRoot((string?)folderPath);

        var rootFolder = existingSourceFolders.SingleOrDefault(sf => sf.IsRoot && sf.Header == root);
        if (rootFolder == null)
        {
            rootFolder = new SourceFolder(root, root, true);
            existingSourceFolders.Add(rootFolder);
        }

        var sourceFolder = new SourceFolder(folderName, fullPath, false);
        rootFolder.Items.Add(sourceFolder);

        return existingSourceFolders.ToObservableCollection();
    }
}


// var fileCount = fullPath.GetFilesByExtension(Extensions.DefaultExtensions);
// sourceFolder.Items.Add(new SourceFolder(fileCount.Count.ToString()));
