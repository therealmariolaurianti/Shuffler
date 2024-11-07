using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Client.Factories;

public class RandomSongQueueState
{
    public RandomSongQueueState(Song? currentSong, ObservableCollection<Song> songs, SongStack songStack,
        bool playingPrevious, bool isSourceGrid)
    {
        CurrentSong = currentSong;
        Songs = songs;
        SongStack = songStack;
        PlayingPrevious = playingPrevious;
        IsSourceGrid = isSourceGrid;
    }

    public Song? CurrentSong { get; }
    public ObservableCollection<Song> Songs { get; }
    public SongStack SongStack { get; }
    public bool PlayingPrevious { get; set; }
    public bool IsSourceGrid { get; }
}

public class RandomSongQueueFactory
{
    public ISongQueue Create(RandomSongQueueState state)
    {
        var newCurrentSong = state.CurrentSong;
        if (!state.PlayingPrevious)
        {
            if (!state.IsSourceGrid)
                newCurrentSong = GetRandomSong(state.CurrentSong, state);
            if (newCurrentSong is not null)
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