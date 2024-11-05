using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class RandomSongQueueState
{
    public RandomSongQueueState(Song? currentSong, ObservableCollection<Song> songs, SongStack songStack,
        bool playingPrevious)
    {
        CurrentSong = currentSong;
        Songs = songs;
        SongStack = songStack;
        PlayingPrevious = playingPrevious;
    }

    public Song? CurrentSong { get; }
    public ObservableCollection<Song> Songs { get; }
    public SongStack SongStack { get; }
    public bool PlayingPrevious { get; set; }
}

public class RandomSongQueueFactory
{
    public ISongQueue Create(RandomSongQueueState state)
    {
        var newCurrentSong = state.CurrentSong;
        if (!state.PlayingPrevious)
        {
            newCurrentSong = GetRandomSong(state.CurrentSong, state);
            state.SongStack.Stack.Add(newCurrentSong);
        }

        var songQueue = new SongQueue
        {
            CurrentSong = newCurrentSong,
            PreviousSong = GetPreviousSong(newCurrentSong, state),
            NextSong = GetRandomSong(newCurrentSong, state)
        };

        return songQueue;
    }

    private Song GetRandomSong(Song? currentSong, RandomSongQueueState state)
    {
        var random = new Random();
        var item = state.Songs.Where(s => s.Id != currentSong?.Id).OrderBy(_ => random.Next()).First();
        if (item == currentSong)
            GetRandomSong(item, state);

        return item;
    }

    private Song? GetPreviousSong(Song? newCurrentSong, RandomSongQueueState state)
    {
        return state.SongStack.GetPrevious(newCurrentSong);
    }
}