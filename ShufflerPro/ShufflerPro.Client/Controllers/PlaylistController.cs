using System.Collections.ObjectModel;
using LiteDB;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.States;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class PlaylistController(DatabaseController databaseController)
{
    public ObservableCollection<Song> IndexSongs(Playlist playlist, List<Song> songs)
    {
        var orderedSongs = new ObservableCollection<Song>();
        var orderedIndexes = playlist.Indexes.OrderBy(i => i.Index).ToList();

        foreach (var songIndex in orderedIndexes)
        {
            var song = songs.SingleOrDefault(s => s.Id == songIndex.SongId);
            if (song is null)
                playlist.Indexes.Remove(songIndex);
            else
                orderedSongs.Add(song);
        }

        return orderedSongs;
    }

    public async Task<NewResult<NewUnit>> AddSongs(Playlist playlist, List<Song> songs)
    {
        foreach (var song in songs)
        {
            var result = await AddSong(playlist, song);
            if (result.Fail)
                return result;
        }

        return await NewUnit.DefaultAsync;
    }

    public async Task<NewResult<NewUnit>> AddSong(Playlist playlist, Song song, int? index = null)
    {
        if (playlist.Indexes.Any(i => i.SongId == song.Id))
            return await NewUnit.DefaultAsync;

        var playlistIndex = CreatePlaylistIndex(playlist, song, index);
        return await databaseController.AddPlaylistIndex(playlistIndex);
    }

    private static PlaylistIndex CreatePlaylistIndex(Playlist playlist, Song song, int? index)
    {
        var playlistIndex =
            new PlaylistIndex(ObjectId.NewObjectId(), song.Id, index ?? playlist.SongCount, playlist.Id);
        playlist.Indexes.Add(playlistIndex);
        return playlistIndex;
    }

    public async Task<NewResult<NewUnit>> AddPlaylist(Library library, Playlist playlist)
    {
        library.Playlists.Add(playlist);
        return await AddPlaylist(playlist);
    }

    private async Task<NewResult<NewUnit>> AddPlaylist(Playlist playlist)
    {
        return await databaseController.AddPlaylist(playlist);
    }

    public async Task<NewResult<NewUnit>> Update(Playlist item)
    {
        return await databaseController.UpdatePlaylist(item);
    }

    public async Task<NewResult<NewUnit>> Delete(Library library, Playlist item)
    {
        library.Playlists.Remove(item);
        return await databaseController.DeletePlaylist(item);
    }

    public async Task<NewResult<NewUnit>> RemoveSong(Playlist playlist, PlaylistState playlistState,
        Song selectedSong)
    {
        playlistState.Songs?.Remove(selectedSong);
        return await DeletePlaylistIndex(playlist, selectedSong);
    }

    private async Task<NewResult<NewUnit>> DeletePlaylistIndex(Playlist playlist, Song selectedSong)
    {
        var playlistIndex = playlist.Indexes.Single(i => i.SongId == selectedSong.Id);
        playlist.Indexes.Remove(playlistIndex);

        return await databaseController.RemovePlaylistIndex(playlistIndex);
    }

    public async Task<NewResult<NewUnit>> MoveSong(PlaylistState playlistState, Song source, int targetIndex)
    {
        return await NewResultExtensions.Try(() =>
            {
                var oldIndex = playlistState.Songs!.IndexOf(source);
                playlistState.Songs!.Move(oldIndex, targetIndex);

                foreach (var playlistStateSong in playlistState.Songs)
                {
                    var playlistIndex = playlistState.Indexes.Single(i => i.SongId == playlistStateSong.Id);
                    playlistIndex.SetIndex(playlistState.Songs.IndexOf(playlistStateSong));
                }

                return NewUnit.Default;
            })
            .Bind(async _ => await databaseController.UpdatePlaylistIndexes(playlistState.Indexes));
    }
}