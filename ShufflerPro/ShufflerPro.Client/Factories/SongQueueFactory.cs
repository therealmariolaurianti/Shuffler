using System.Collections.ObjectModel;
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
        var index = createdAlbumSongs.IndexOf(currentSong) - 1;
        return index == -1 ? null : createdAlbumSongs.Skip(index).FirstOrDefault();
    }
}