using System.IO;
using ShufflerPro.Upgraded.Bootstrapper;
using ShufflerPro.Upgraded.Factories;
using ShufflerPro.Upgraded.Objects;

namespace ShufflerPro.Upgraded.Controllers;

public class MediaController(ArtistFactory artistFactory, AlbumFactory albumFactory)
{
    private readonly List<string> _mediaLibraryPaths = ["X:\\"];

    public IReadOnlyCollection<Artist> LoadArtists()
    {
        var allArtists = _mediaLibraryPaths.SelectMany(path => Process(LoadSongsInPath(path)));
        return allArtists.ToReadOnlyCollection();
    }

    private static List<Song> LoadSongsInPath(string mediaLibraryPath)
    {
        var songsPaths = Path.GetFullPath(mediaLibraryPath)
            .GetFilesByExtension(["mp3", ".m4a"])
            .Take(100)
            .ToHashSet();

        return songsPaths.AsParallel().Select(SongFactory.Create).ToList();
    }

    private List<Artist> Process(List<Song> songs)
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

        return artists;
    }
}