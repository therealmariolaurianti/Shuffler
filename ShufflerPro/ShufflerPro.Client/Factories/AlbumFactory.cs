using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Factories;

public class AlbumFactory
{
    public Album Create(string artist, string name, List<Song> songs)
    {
        var album = new Album(artist, name, songs);
        songs.ForEach(s => s.CreatedAlbum = album);
        return album;
    }
}