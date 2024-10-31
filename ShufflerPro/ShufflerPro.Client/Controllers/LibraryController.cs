using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Factories;
using ShufflerPro.Database;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class LibraryController
{
    private readonly LibraryFactory _libraryFactory;
    private readonly LocalDatabase _localDatabase;
    private readonly string _localDatabasePath;
    private readonly MediaController _mediaController;
    private readonly SourceFolderController _sourceFolderController;

    public LibraryController(
        LocalDatabase localDatabase,
        SourceFolderController sourceFolderController,
        LibraryFactory libraryFactory,
        MediaController mediaController)
    {
        _localDatabase = localDatabase;
        _sourceFolderController = sourceFolderController;
        _libraryFactory = libraryFactory;
        _mediaController = mediaController;

        //move to initialize method to capture if root isnt found exception
        var root = FindRoot();
        _localDatabasePath = $@"{root}\local.db";
    }

    public async Task<NewResult<Library>> Initialize()
    {
        using (var connection = _localDatabase.CreateConnection(_localDatabasePath))
        {
            return await LoadSourcesFromDatabase(connection)
                .Bind(sourcePaths => _sourceFolderController
                    .BuildSourceFolders(sourcePaths, new List<SourceFolder>())
                    .Bind(sourceFolders =>
                    {
                        var library = _libraryFactory.Create(sourceFolders);
                        return _mediaController
                            .LoadFromFolderPath(sourceFolders, library)
                            .Map(_ => library);
                    }));
        }
    }

    private static async Task<NewResult<List<string>>> LoadSourcesFromDatabase(LocalDatabaseConnection connection)
    {
        var sources = connection.GetCollection<Source>();
        var sourcePaths = await sources.FindAll().ConfigureAwait(true);

        return sourcePaths.Select(s => s.FolderPath).ToList();
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

    public async Task<NewResult<NewUnit>> SaveFolderPath(string folderPath)
    {
        return await NewResultExtensions.Try(async () =>
        {
            using (var connection = _localDatabase.CreateConnection(_localDatabasePath))
            {
                var source = new Source(folderPath);
                var sourceCollection = connection.GetCollection<Source>();
                await sourceCollection.Insert(source);
            }

            return NewUnit.Default;
        });
    }
}