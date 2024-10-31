using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Factories;
using ShufflerPro.Database;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class LibraryController
{
    private readonly LibraryFactory _libraryFactory;
    private readonly LocalDatabase _localDatabase;
    private readonly MediaController _mediaController;
    private readonly SourceFolderController _sourceFolderController;

    public LibraryController(LocalDatabase localDatabase, SourceFolderController sourceFolderController,
        LibraryFactory libraryFactory, MediaController mediaController)
    {
        _localDatabase = localDatabase;
        _sourceFolderController = sourceFolderController;
        _libraryFactory = libraryFactory;
        _mediaController = mediaController;
    }

    public async Task<NewResult<Library>> LoadLibrary()
    {
        var root = FindRoot();
        var localDatabasePath = $@"{root}\local.db";

        using (var connection = _localDatabase.CreateConnection(localDatabasePath))
        {
            return await LoadSourcesFromDatabase(connection)
                .Bind(sourcePaths => _sourceFolderController
                    .BuildSourceFolders(sourcePaths, new List<SourceFolder>())
                    .Bind(sourceFolders =>
                    {
                        var library = _libraryFactory.Create();
                        return _mediaController
                            .LoadFromFolderPath(sourceFolders, library)
                            .Map(_ => library);
                    }));
        }
    }

    private static async Task<NewResult<List<string>>> LoadSourcesFromDatabase(LocalDatabaseConnection connection)
    {
        var sources = connection.GetCollection<Source>();
        var sourcePaths = sources.FindAll().Select(s => s.Path).ToList();
        return sourcePaths;
    }

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