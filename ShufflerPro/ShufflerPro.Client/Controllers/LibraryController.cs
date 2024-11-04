using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Factories;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class LibraryController
{
    private readonly DatabaseController _databaseController;
    private readonly LibraryFactory _libraryFactory;
    private readonly MediaController _mediaController;
    private readonly SourceFolderController _sourceFolderController;

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
                .BuildFromSources(sourcePaths, new SourceFolderState(new List<SourceFolder>()))
                .Bind(state =>
                {
                    var library = _libraryFactory.Create(state.SourceFolders);
                    return _mediaController
                        .LoadFromFolderPath(state.SourceFolders, library)
                        .Map(_ => library);
                }));
    }

    public async Task<NewResult<NewUnit>> InsertSource(SourceFolderState state)
    {
        return await _databaseController.InsertSource(state);
    }
}