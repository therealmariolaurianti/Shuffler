using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Controllers;

public class PlaylistController
{
    public void Initialize(Library library)
    {
        var playlist = new Playlist("Favorites");

        library.Playlists.Add(playlist);
    }

    public ObservableCollection<Song> IndexSongs(Playlist playlist, List<Song> songs)
    {
        var orderedSongs = new ObservableCollection<Song>();
        foreach (var songIndex in playlist.SongIndex.Index)
        {
            var song = songs.Single(s => s.Id == songIndex.Key);
            orderedSongs.Insert(songIndex.Value, song);
        }

        return orderedSongs;
    }

    public void AddSong(Playlist playlist, Song song)
    {
        if (playlist.Songs.Contains(song))
            return;

        var index = playlist.Songs.Count;

        playlist.SongIndex.Index.Add(song.Id, index);
        playlist.Songs.Add(song);
    }
}