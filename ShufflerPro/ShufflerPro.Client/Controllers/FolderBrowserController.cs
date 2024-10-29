using System.Collections.ObjectModel;

namespace ShufflerPro.Client.Controllers;

public class FolderBrowserController(MediaController mediaController)
{
    private MediaController _mediaController = mediaController;

    // public void Add(ObservableCollection<SourceFolder> sourceFolders)
    // {
    //     using (var fbd = new FolderBrowserDialog())
    //     {
    //         var result = fbd.ShowDialog();
    //
    //         if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
    //         {
    //             var folderName = Path.GetFileName((string?)fbd.SelectedPath);
    //             var fullPath = Path.GetFullPath(fbd.SelectedPath);
    //
    //             var root = Path.GetPathRoot((string?)fbd.SelectedPath);
    //             var rootFolder = sourceFolders.SingleOrDefault(sf => sf.IsRoot && (string?)sf.Header == root);
    //             if (rootFolder is null)
    //             {
    //                 rootFolder = new SourceFolder(root, root, true);
    //                 sourceFolders.Add(rootFolder);
    //             }
    //
    //             var sourceFolder = new SourceFolder(folderName, fullPath, false);
    //
    //             var fileCount = fullPath.GetFilesByExtension(Extensions.Extensions.DefaultExtensions);
    //             sourceFolder.Items.Add(new SourceFolder(fileCount.Count.ToString()));
    //
    //             rootFolder.Items.Add(sourceFolder);
    //         }
    //     }
    // }
}