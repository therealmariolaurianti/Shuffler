using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;
using ShufflerPro.Client.Factories;
using ShufflerPro.Client.States;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class LibraryController
{
    private readonly DatabaseController _databaseController;
    private readonly LibraryFactory _libraryFactory;
    private readonly MediaController _mediaController;
    private readonly SongController _songController;
    private readonly SourceFolderController _sourceFolderController;

    public LibraryController(
        SourceFolderController sourceFolderController,
        LibraryFactory libraryFactory,
        MediaController mediaController,
        DatabaseController databaseController, SongController songController)
    {
        _sourceFolderController = sourceFolderController;
        _libraryFactory = libraryFactory;
        _mediaController = mediaController;
        _databaseController = databaseController;
        _songController = songController;
    }

    public async Task<NewResult<Library>> Initialize()
    {
        return await _databaseController.LoadSources()
            .Bind(sourcePaths => _sourceFolderController
                .BuildFromSources(sourcePaths, new SourceFolderState(new List<SourceFolder>()))
                .Bind(async state =>
                {
                    var library = _libraryFactory.Create(state.SourceFolders);
                    return await LoadExcludeSongs(library)
                        .Bind(excludedSongs => _mediaController
                            .LoadFromFolderPath(state.SourceFolders, library, excludedSongs))
                        .Map(async _ => await LoadPlaylists(library))
                        .Map(_ => library);
                }));
    }

    private async Task<NewResult<List<ExcludedSong>>> LoadExcludeSongs(Library library)
    {
        return await _databaseController
            .LoadExcludedSongs()
            .Do(excludedSongs => library.ExcludedSongs = excludedSongs);
    }

    public async Task<NewResult<NewUnit>> LoadPlaylists(Library library)
    {
        return await _databaseController
            .LoadPlaylists()
            .Do(playlists => library.Playlists.AddRange(playlists))
            .ToSuccessAsync();
    }

    public async Task<NewResult<NewUnit>> InsertSource(SourceFolderState state)
    {
        return await _databaseController.InsertSource(state);
    }
}