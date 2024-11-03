using System.Collections.ObjectModel;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class SongQueueFactory
{
    public SongQueue Create(Song currentSong, ObservableCollection<Song>? songs)
    {
        return new SongQueue
        {
            CurrentSong = currentSong,
            PreviousSong = GetPreviousSong(currentSong, songs),
            NextSong = GetNextSong(currentSong, songs)
        };
    }

    private static Song? GetNextSong(Song currentSong, ObservableCollection<Song> createdAlbumSongs)
    {
        var index = createdAlbumSongs.IndexOf(currentSong) + 1;
        var nextSong = createdAlbumSongs.Skip(index).FirstOrDefault();
        
        return nextSong;
    }

    private static Song? GetPreviousSong(Song currentSong, ObservableCollection<Song> createdAlbumSongs)
    {
        var index1 = createdAlbumSongs.IndexOf(currentSong) - 1;
        var previousSong = createdAlbumSongs.Skip(index1).FirstOrDefault();
        
        return previousSong;
    }
}