using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;
using ShufflerPro.Client.Factories;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class MediaController(ArtistFactory artistFactory, AlbumFactory albumFactory)
{
    public IReadOnlyCollection<Artist> LoadFromFolderPath(string folderPath)
    {
        return Process(LoadSongsInPath(folderPath));
    }

    private static List<Song> LoadSongsInPath(string mediaLibraryPath)
    {
        var songsPaths = Path.GetFullPath(mediaLibraryPath)
            .GetFilesByExtension(Extensions.Extensions.DefaultExtensions)
            .ToHashSet();

        return songsPaths.AsParallel().Select(SongFactory.Create).ToList();
    }

    private ReadOnlyCollection<Artist> Process(List<Song> songs)
    {
        var songFilesWithArtists = songs
            .Distinct()
            .Where(s => !string.IsNullOrEmpty(s.Artist))
            .ToList();

        var albums = songFilesWithArtists
            .GroupBy(song => song.Album)
            .ToDictionary(s => s.Key, g => g.ToList());

        var albumList = new List<Album>();
        foreach (var kvp in albums)
        {
            var artist = kvp.Value.First().Artist;
            var album = albumFactory.Create(artist, kvp.Key, kvp.Value);
            albumList.Add(album);
        }

        var groupedAlbums = albumList
            .GroupBy(a => a.Artist)
            .ToDictionary(s => s.Key, g => g.ToList());

        var artists = groupedAlbums
            .Select(album => artistFactory.Create(album.Key, album.Value)).OrderBy(a => a.Name)
            .ToList();

        return artists.ToReadOnlyCollection();
    }

    public NewResult<Library> LoadLibrary(Guid library)
    {
        return new NewResult<Library>();
    }
}