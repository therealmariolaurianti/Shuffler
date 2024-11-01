using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class SongQueueFactory
{
    public SongQueue Create(Song currentSong)
    {
        var buildSongList = new SongQueue
        {
            CurrentSong = currentSong
        };

        var createdAlbumSongs = currentSong.CreatedAlbum!.Songs;

        buildSongList.PreviousSong = GetPreviousSong(currentSong, createdAlbumSongs);
        buildSongList.NextSong = GetNextSong(currentSong, createdAlbumSongs);

        return buildSongList;
    }

    private static Song? GetNextSong(Song currentSong, List<Song> createdAlbumSongs)
    {
        var index = createdAlbumSongs.IndexOf(currentSong) + 1;
        var nextSong = createdAlbumSongs.Skip(index).FirstOrDefault();
        if (nextSong is null)
        {
            var albumIndex = currentSong.CreatedAlbum.CreatedArtist.Albums.IndexOf(currentSong.CreatedAlbum) + 1;
            var nextAlbum = currentSong.CreatedAlbum.CreatedArtist.Albums.Skip(albumIndex).FirstOrDefault();
            if (nextAlbum != null)
                return nextAlbum.Songs.FirstOrDefault();
        }
        
        return nextSong;
    }

    private static Song? GetPreviousSong(Song currentSong, List<Song> createdAlbumSongs)
    {
        var index1 = createdAlbumSongs.IndexOf(currentSong) - 1;
        var previousSong = createdAlbumSongs.Skip(index1).FirstOrDefault();
        if (previousSong is null || index1 == -1)
        {
            var albumIndex = currentSong.CreatedAlbum.CreatedArtist.Albums.IndexOf(currentSong.CreatedAlbum) - 1;
            var previousAlbum = currentSong.CreatedAlbum.CreatedArtist.Albums.Skip(albumIndex).FirstOrDefault();
            if (previousAlbum != null)
                return previousAlbum.Songs.LastOrDefault();
        }
        
        return previousSong;
    }
}