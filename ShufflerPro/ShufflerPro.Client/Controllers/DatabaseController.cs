using LiteDB;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Client.States;
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
            var playlistIndexCollection = connection.GetCollection<PlaylistIndex>();

            var playlists = (await playlistCollection.FindAll().ConfigureAwait(true)).ToList();
            var playlistIndexes = await playlistIndexCollection.FindAll().ConfigureAwait(true);

            var indexDic = playlistIndexes.GroupBy(pi => pi.PlaylistId!)
                .ToDictionary(d => d.Key, d => d.ToList());

            foreach (var (objectId, playlistIndices) in indexDic)
            {
                var playlist = playlists.Single(p => p.Id == objectId);
                playlist.Indexes.AddRange(playlistIndices);
            }

            return playlists.ToList();
        }
    }

    public async Task<NewResult<NewUnit>> AddPlaylist(Playlist playlist)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var playlistCollection = connection.GetCollection<Playlist>();
            var localDatabaseKey = await playlistCollection.Insert(playlist);

            playlist.SetId(localDatabaseKey.AsBsonValue());
        }

        return NewUnit.Default;
    }

    public async Task<NewResult<NewUnit>> AddPlaylistIndex(PlaylistIndex playlistIndex)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var playlistCollection = connection.GetCollection<PlaylistIndex>();
            var localDatabaseKey = await playlistCollection.Insert(playlistIndex);

            playlistIndex.SetId(localDatabaseKey.AsBsonValue());
        }

        return NewUnit.Default;
    }

    public async Task<NewResult<NewUnit>> UpdatePlaylist(Playlist item)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var playlistCollection = connection.GetCollection<Playlist>();
            var result = await playlistCollection.Update(item);
            if (!result)
                return NewResultExtensions.CreateFail<NewUnit>($"Failed to update {item.Name}");
        }

        return NewUnit.Default;
    }

    public async Task<NewResult<NewUnit>> DeletePlaylist(Playlist item)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var playlistCollection = connection.GetCollection<Playlist>();
            var playlistIndexCollection = connection.GetCollection<PlaylistIndex>();

            foreach (var index in item.Indexes)
                await playlistIndexCollection.Delete(new LocalDatabaseKey(index.Id));

            var result = await playlistCollection.Delete(new LocalDatabaseKey(item.Id));
            if (!result)
                return NewResultExtensions.CreateFail<NewUnit>($"Failed to delete {item.Name}");
        }

        return NewUnit.Default;
    }

    public async Task<NewResult<NewUnit>> RemovePlaylistIndex(PlaylistIndex playlistIndex)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var playlistCollection = connection.GetCollection<PlaylistIndex>();
            var result = await playlistCollection.Delete(new LocalDatabaseKey(playlistIndex.Id));
            if (!result)
                return NewResultExtensions.CreateFail<NewUnit>($"Failed to delete {playlistIndex}");
        }

        return await NewUnit.DefaultAsync;
    }

    public ISettings LoadSettings()
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var settingsCollection = connection.GetCollection<Settings>();
            var settings = Task.Run(async () => await settingsCollection.FindAll()).Result.ToList();
            if (!settings.Any())
            {
                var createdSettings = new Settings
                {
                    IsDarkModeEnabled = true
                };

                var id = Task.Run(async () => await settingsCollection.Insert(createdSettings)).Result;
                createdSettings.Id = id.AsBsonValue();

                return createdSettings;
            }

            return settings.First();
        }
    }

    public async Task<NewResult<NewUnit>> UpdateSettings(Settings settings)
    {
        using (var connection = _localDatabase.CreateConnection(_databasePath.Path))
        {
            var settingsCollection = connection.GetCollection<Settings>();
            var result = await settingsCollection.Update(settings);
            if (!result)
                return NewResultExtensions.CreateFail<NewUnit>("Failed to update settings.");
        }

        return NewUnit.Default;
    }
}