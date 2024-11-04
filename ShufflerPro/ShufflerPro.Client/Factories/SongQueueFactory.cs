using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class SongStack
{
    public Queue<Song> Stack { get; set; } = new();
}

public class RandomSongQueueFactory
{
    public ISongQueue Create(Song? currentSong, ObservableCollection<Song> songs, SongStack songStack)
    {
        return new SongQueue
        {
            CurrentSong = currentSong ?? GetRandomSong(songs, songStack),
            PreviousSong = GetPreviousSong(songStack),
            NextSong = GetRandomSong(songs, songStack)
        };
    }

    private Song GetRandomSong(ObservableCollection<Song> songs, SongStack songStack)
    {
        var random = new Random();
        var index = random.Next(songs.Count);
        var nextSong = songs[index];

        songStack.Stack.Enqueue(nextSong);

        return nextSong;
    }

    private Song? GetPreviousSong(SongStack songStack)
    {
        songStack.Stack.TryDequeue(out var previousSong);
        return previousSong;
    }
}

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