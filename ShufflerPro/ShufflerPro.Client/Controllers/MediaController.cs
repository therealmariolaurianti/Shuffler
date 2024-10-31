using System.Collections.ObjectModel;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Factories;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class MediaController(
    ArtistFactory artistFactory,
    AlbumFactory albumFactory)
{
    public NewResult<NewUnit> LoadFromFolderPath(ICollection<SourceFolder> sourceFolders,
        Library library)
    {
        return NewResultExtensions.Try(() =>
        {
            var roots = sourceFolders.Where(sf => sf.IsRoot);
            foreach (var root in roots)
                ProcessPath(library, root);

            return NewUnit.Default;
        });
    }

    private void ProcessPath(Library library, SourceFolder sourceFolder)
    {
        if (sourceFolder is { IsRoot: false, IsProcessed: false, Items.Count: 0 })
        {
            var path = sourceFolder.FullPath;
            var loadSongsInPath = LoadSongsInPath(path);
            var readOnlyCollection = Process(loadSongsInPath);

            library.AddArtists(readOnlyCollection);
        }

        sourceFolder.IsProcessed = true;

        foreach (var folderItem in sourceFolder.Items)
            ProcessPath(library, folderItem);
    }

    private static List<Song> LoadSongsInPath(string mediaLibraryPath)
    {
        var songsPaths = Path.GetFullPath(mediaLibraryPath).GetFilesByExtension()
            .ToHashSet();

        return songsPaths.Select(SongFactory.Create).ToList();
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
}