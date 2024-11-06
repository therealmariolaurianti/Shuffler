using LiteDB;
using ShufflerPro.Client.Entities;
using ShufflerPro.Database;
using ShufflerPro.Database.Interfaces;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class DatabaseController
{
    private readonly IDatabasePath _databasePath;
    private readonly LocalDatabase _localDatabase;

    public DatabaseController(LocalDatabase localDatabase, IDatabasePath databasePath)
    {
        _localDatabase = localDatabase;
        _databasePath = databasePath;
    }

    public async Task<NewResult<List<Source>>> LoadSources()
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var sources = connection.GetCollection<Source>();
            var sourcePaths = await sources.FindAll().ConfigureAwait(true);

            return sourcePaths.ToList();
        }
    }

    public async Task<NewResult<NewUnit>> InsertSource(SourceFolderState state)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            foreach (var addedSourceFolder in state.AddedSourceFolders)
            {
                var sourceCollection = connection.GetCollection<Source>();

                var source = new Source(addedSourceFolder.FullPath, ObjectId.NewObjectId());
                var localDatabaseKey = await sourceCollection.Insert(source);

                addedSourceFolder.SetId(localDatabaseKey);
            }
        }

        return NewUnit.Default;
    }

    public async Task<NewResult<NewUnit>> DeleteSource(SourceFolder sourceFolder)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var sourceCollection = connection.GetCollection<Source>();
            foreach (var folder in sourceFolder.Flatten())
                if (folder.Id is not null)
                    await sourceCollection.Delete(folder.Id);
        }

        return NewUnit.Default;
    }

    public async Task<NewResult<List<Playlist>>> LoadPlaylists()
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var playlistCollection = connection.GetCollection<Playlist>();
            return (await playlistCollection.FindAll()).ToList();
        }
    }

    public async Task<NewResult<NewUnit>> SavePlaylist(Playlist playlist)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var playlistCollection = connection.GetCollection<Playlist>();
            var localDatabaseKey = await playlistCollection.Insert(playlist);

            playlist.SetId(localDatabaseKey);
        }

        return NewUnit.Default;
    }
}