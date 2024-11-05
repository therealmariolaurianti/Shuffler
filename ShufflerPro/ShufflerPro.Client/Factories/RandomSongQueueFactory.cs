using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class RandomSongQueueFactory
{
    public ISongQueue Create(Song? currentSong, ObservableCollection<Song> songs, SongStack songStack)
    {
        var songQueue = new SongQueue
        {
            CurrentSong = GetRandomSong(currentSong, songs),
            PreviousSong = GetPreviousSong(currentSong, songStack),
            NextSong = GetRandomSong(currentSong, songs)
        };
        
        songStack.Stack.Add(songQueue.CurrentSong);
        
        return songQueue;
    }

    private Song GetRandomSong(Song? currentSong, ObservableCollection<Song> songs)
    {
        var random = new Random();
        var index = random.Next(songs.Count);
        var nextSong = songs[index];
        
        if(currentSong == nextSong)
            GetRandomSong(currentSong, songs);

        return nextSong;
    }

    private Song? GetPreviousSong(Song? currentSong, SongStack songStack)
    {
        var index = songStack.Stack.IndexOf(currentSong) - 1;
        var previousSong = songStack.Stack.Skip(index).FirstOrDefault();
        return previousSong;
    }
}