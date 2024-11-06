using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class PlaylistController(DatabaseController databaseController)
{
    public async Task<NewResult<NewUnit>> Initialize(Library library)
    {
        return await databaseController
            .LoadPlaylists()
            .Do(playlists => library.Playlists.AddRange(playlists))
            .ToSuccessAsync();
    }

    public ObservableCollection<Song> IndexSongs(Playlist playlist, List<Song> songs)
    {
        var orderedSongs = new ObservableCollection<Song>();
        foreach (var songIndex in playlist.Indexes)
        {
            var song = songs.Single(s => s.Id == songIndex.Id);
            orderedSongs.Insert(songIndex.Index, song);
        }

        return orderedSongs;
    }

    public void AddSong(Playlist playlist, Song song)
    {
        if (playlist.Indexes.Any(i => i.Id == song.Id))
            return;
        playlist.Indexes.Add(new PlaylistIndex(song.Id, playlist.SongCount));
    }

    public async Task<NewResult<NewUnit>> AddPlaylist(Library library, Playlist playlist)
    {
        library.Playlists.Add(playlist);
        return await SavePlaylist(playlist);
    }

    private async Task<NewResult<NewUnit>> SavePlaylist(Playlist playlist)
    {
        return await databaseController.SavePlaylist(playlist);
    }
}