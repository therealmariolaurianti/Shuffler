using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Enums;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Client.States;

namespace ShufflerPro.Client.Factories;

public class SongQueueFactory
{
    public ISongQueue Create(Song? currentSong, ObservableCollection<Song>? songs, RepeatState repeatState)
    {
        return new SongQueue
        {
            CurrentSong = currentSong ?? songs.FirstOrDefault(),
            PreviousSong = GetPreviousSong(currentSong, songs),
            NextSong = GetNextSong(currentSong, songs, repeatState)
        };
    }

    private static Song? GetNextSong(Song? currentSong, ObservableCollection<Song>? createdAlbumSongs,
        RepeatState repeatState)
    {
        if (currentSong is null)
            return null;

        if (repeatState is { IsRepeatChecked: true, RepeatType: RepeatType.Song })
            return currentSong;

        var index = createdAlbumSongs.IndexOf(currentSong) + 1;
        var nextSong = createdAlbumSongs.Skip(index).FirstOrDefault();

        if (nextSong is null)
        {
            if (!repeatState.IsRepeatChecked)
                return nextSong;

            if (repeatState.RepeatType == RepeatType.Album)
                return createdAlbumSongs.First();

            if (repeatState.RepeatType == RepeatType.Artist)
            {
                var nextAlbumIndex = currentSong.CreatedAlbum!.Artist.Albums.IndexOf(currentSong.CreatedAlbum);
                var nextAlbum = currentSong.CreatedAlbum.Artist.Albums.ElementAtOrDefault(nextAlbumIndex + 1);
                return nextAlbum is null 
                    ? currentSong.CreatedAlbum.Artist.Albums.FirstOrDefault()?.Songs.FirstOrDefault() 
                    : nextAlbum.Songs.FirstOrDefault();
            }
        }

        return nextSong;
    }

    private static Song? GetPreviousSong(Song? currentSong, ObservableCollection<Song>? createdAlbumSongs)
    {
        if (currentSong is null)
            return null;

        var index = createdAlbumSongs.IndexOf(currentSong) - 1;
        return index == -1 ? null : createdAlbumSongs.Skip(index).FirstOrDefault();
    }
}