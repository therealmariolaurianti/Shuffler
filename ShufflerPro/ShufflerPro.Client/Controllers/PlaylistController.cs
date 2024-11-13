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

    public async Task<NewResult<NewUnit>> AddSong(Playlist playlist, Song song)
    {
        if (playlist.Indexes.Any(i => i.SongId == song.Id))
            return await NewUnit.DefaultAsync;
        var playlistIndex = new PlaylistIndex(ObjectId.NewObjectId(), song.Id, playlist.SongCount, playlist.Id);
        playlist.Indexes.Add(playlistIndex);

        return await databaseController.AddPlaylistIndex(playlistIndex);
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

    public async Task<NewResult<NewUnit>> RemoveSong(Playlist playlist, PlaylistState? playlistState,
        Song selectedSong)
    {
        playlistState?.Songs?.Remove(selectedSong);

        var playlistIndex = playlist.Indexes.Single(i => i.SongId == selectedSong.Id);
        playlist.Indexes.Remove(playlistIndex);

        return await databaseController.RemovePlaylistIndex(playlistIndex);
    }
}