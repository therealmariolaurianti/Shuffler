using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class SongQueueFactory
{
    public ISongQueue Create(Song? currentSong, ObservableCollection<Song> songs)
    {
        return new SongQueue
        {
            CurrentSong = currentSong ?? songs.FirstOrDefault(),
            PreviousSong = GetPreviousSong(currentSong, songs),
            NextSong = GetNextSong(currentSong, songs)
        };
    }

    private static Song? GetNextSong(Song? currentSong, ObservableCollection<Song> createdAlbumSongs)
    {
        if (currentSong is null)
            return null;

        var index = createdAlbumSongs.IndexOf(currentSong) + 1;
        var nextSong = createdAlbumSongs.Skip(index).FirstOrDefault();

        return nextSong;
    }

    private static Song? GetPreviousSong(Song? currentSong, ObservableCollection<Song> createdAlbumSongs)
    {
        if (currentSong is null)
            return null;

        var index = createdAlbumSongs.IndexOf(currentSong) - 1;
        return index == -1 ? null : createdAlbumSongs.Skip(index).FirstOrDefault();
    }
}