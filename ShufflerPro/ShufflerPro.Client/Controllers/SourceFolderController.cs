using ShufflerPro.Client.Entities;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class SourceFolderController(DatabaseController databaseController)
{
    public NewResult<SourceFolderState> BuildFromSources(List<Source> sources, SourceFolderState state)
    {
        foreach (var source in sources)
        {
            var result = BuildFromPath(source.FolderPath, state);
            if (result.Fail)
                return result;
        }

        return state;
    }

    public NewResult<SourceFolderState> BuildFromPath(string folderPath, SourceFolderState state)
    {
        return FindRoot(folderPath, state.SourceFolders)
            .Map(rootFolder =>
            {
                var allFolders = new List<SourceFolder> { rootFolder };
                
                FindAllFolders(rootFolder, allFolders);

                var addedSourceFolders = new List<SourceFolder>();
                var levels = folderPath.Split(Path.DirectorySeparatorChar).ToList();
                
                Build(rootFolder.Header, levels, rootFolder, allFolders, folderPath, addedSourceFolders);

                state.AddedSourceFolders.AddRange(addedSourceFolders);
                return state;
            });
    }

    public async Task<NewResult<NewUnit>> Remove(Library library, SourceFolder sourceFolder)
    {
        return await RemoveFolderFromSourceCollection(library, sourceFolder)
            .Bind(_ => RemoveSongsFromLibrary(library, sourceFolder))
            .Bind(async _ => await RemoveFolderFromDatabase(sourceFolder));
    }

    private async Task<NewResult<NewUnit>> RemoveFolderFromDatabase(SourceFolder sourceFolder)
    {
        var sourceFolderContents = sourceFolder.Flatten();
        foreach (var folder in sourceFolderContents)
        {
            await databaseController.DeleteSource(folder);
        }

        return NewUnit.Default;
    }

    private NewResult<NewUnit> RemoveSongsFromLibrary(Library library, SourceFolder sourceFolder)
    {
        return NewResultExtensions.Try(() =>
        {
            foreach (var folder in sourceFolder.Flatten())
            {
                var songs = library.Songs.Where(s => s.Path != null && s.Path.Contains(folder.FullPath));
                foreach (var song in songs)
                    if (song.CreatedAlbum?.Songs.Count - 1 == 0)
                    {
                        song.CreatedAlbum!.Artist.Albums.Remove(song.CreatedAlbum);
                        if (song.CreatedAlbum!.Artist.Albums.Count == 0)
                            library.Artists.Remove(song.CreatedAlbum!.Artist);
                    }
                    else
                    {
                        song.CreatedAlbum?.Songs.Remove(song);
                    }
            }

            return NewUnit.Default;
        });
    }

    private NewResult<NewUnit> RemoveFolderFromSourceCollection(Library library, SourceFolder sourceFolder)
    {
        return NewResultExtensions.Try(() =>
        {
            if (sourceFolder.Parent != null)
                sourceFolder.Parent.Items.Remove(sourceFolder);
            else
                library.SourceFolders.Remove(sourceFolder);

            return NewUnit.Default;
        });
    }

    private static void FindAllFolders(SourceFolder rootFolder, List<SourceFolder> allFolders)
    {
        foreach (var rootFolderItem in rootFolder.Items)
        {
            allFolders.Add(rootFolderItem);
            FindAllFolders(rootFolderItem, allFolders);
        }
    }

    private static void Build(string root, List<string> levels, SourceFolder rootFolder,
        List<SourceFolder> allFolders, string fullPath, List<SourceFolder> addedSourceFolders)
    {
        var lastLevel = levels.Last();
        foreach (var level in levels)
        {
            var currentPath = BuildCurrentPath(levels, level);
            if (level == root)
                continue;

            var existingFolder = allFolders.SingleOrDefault(af => af.Header == level && af.FullPath == currentPath);
            if (existingFolder is not null)
            {
                rootFolder = existingFolder;
                continue;
            }

            var item = new SourceFolder(level, rootFolder, currentPath);
            if (currentPath == fullPath)
                addedSourceFolders.Add(item);

            var existingParentFolder = allFolders.SingleOrDefault(f => f.Header == item.Header && 
                                                                       f.Parent?.Header == item.Parent?.Header);
            if (existingParentFolder is not null)
            {
                rootFolder = existingParentFolder;
            }
            else
            {
                rootFolder.Items.Add(item);
                rootFolder = item;
                
                allFolders.Add(item);
            }

            if (lastLevel == level)
                BuildChildDirectories(root, rootFolder, allFolders, fullPath, addedSourceFolders);
        }
    }

    private static string BuildCurrentPath(List<string> levels, string level)
    {
        var index = levels.IndexOf(level);
        var currentPath = string.Join(Path.DirectorySeparatorChar, levels.Take(index + 1));
        return currentPath;
    }

    private static void BuildChildDirectories(string root, SourceFolder rootFolder, List<SourceFolder> allFolders,
        string fullPath, List<SourceFolder> addedSourceFolders)
    {
        try
        {
            var directories = Directory.GetDirectories(fullPath);
            if (!directories.Any())
                return;
            foreach (var directory in directories)
            {
                var x = directory.Split(Path.DirectorySeparatorChar).ToList();
                Build(root, x, rootFolder, allFolders, directory, addedSourceFolders);
            }
        }
        catch (Exception)
        {
            //ignore
        }
    }

    private NewResult<SourceFolder> FindRoot(string folderPath, ICollection<SourceFolder> existingSourceFolders)
    {
        var rootPath = Path.GetPathRoot((string?)folderPath)?.Replace(Path.DirectorySeparatorChar, ' ').Trim();
        if (rootPath is null)
            return NewResultExtensions.CreateFail<SourceFolder>(new Exception("Failed to load source"));

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