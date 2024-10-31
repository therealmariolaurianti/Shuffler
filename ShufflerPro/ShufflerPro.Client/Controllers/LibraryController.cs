using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Factories;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class LibraryController
{
    private readonly LibraryFactory _libraryFactory;
    private readonly MediaController _mediaController;
    private readonly SourceFolderController _sourceFolderController;
    private readonly DatabaseController _databaseController;

    public LibraryController(
        SourceFolderController sourceFolderController,
        LibraryFactory libraryFactory,
        MediaController mediaController, 
        DatabaseController databaseController)
    {
        _sourceFolderController = sourceFolderController;
        _libraryFactory = libraryFactory;
        _mediaController = mediaController;
        _databaseController = databaseController;
    }

    public async Task<NewResult<Library>> Initialize()
    {
        return await _databaseController.LoadSources()
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


    public async Task<NewResult<NewUnit>> InsertSource(string folderPath)
    {
        return await _databaseController.InsertSource(folderPath);
    }
}