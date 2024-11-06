using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Controllers;

public class PlaylistController
{
    public void Initialize(Library library)
    {
        AddPlaylist(library);
    }

    private void AddPlaylist(Library library)
    {
        library.Playlists.Add(new Playlist("Favorites"));
    }

    public ObservableCollection<Song> IndexSongs(Playlist playlist, List<Song> songs)
    {
        var orderedSongs = new ObservableCollection<Song>();
        foreach (var songIndex in playlist.Index)
        {
            var song = songs.Single(s => s.Id == songIndex.Key);
            orderedSongs.Insert(songIndex.Value, song);
        }

        return orderedSongs;
    }

    public void AddSong(Playlist playlist, Song song)
    {
        if (playlist.Index.ContainsKey(song.Id))
            return;
        playlist.Index.Add(song.Id, playlist.SongCount);
    }
}