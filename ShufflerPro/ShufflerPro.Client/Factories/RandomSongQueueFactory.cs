using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class RandomSongQueueFactory
{
    public ISongQueue Create(Song? currentSong, ObservableCollection<Song> songs, SongStack songStack)
    {
        var newCurrentSong = GetRandomSong(currentSong, songs);
        var songQueue = new SongQueue
        {
            CurrentSong = newCurrentSong,
            PreviousSong = GetPreviousSong(currentSong, songStack),
            NextSong = GetRandomSong(newCurrentSong, songs)
        };

        songStack.Stack.Add(songQueue.CurrentSong);

        return songQueue;
    }

    private Song GetRandomSong(Song? currentSong, ObservableCollection<Song> songs)
    {
        var random = new Random();
        var item = songs.Where(s => s.Id != currentSong?.Id).OrderBy(_ => random.Next()).First();
        if (item == currentSong)
            GetRandomSong(item, songs);

        return item;
    }

    private Song? GetPreviousSong(Song? currentSong, SongStack songStack)
    {
        var index = songStack.Stack.IndexOf(currentSong) - 1;
        var previousSong = songStack.Stack.Skip(index).FirstOrDefault();
        return previousSong;
    }
}