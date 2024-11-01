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

            Process(library, loadSongsInPath);
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

    private void Process(Library library, List<Song> songs)
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

        foreach (var groupedAlbum in groupedAlbums)
        {
            var existingArtist = library.Artists.SingleOrDefault(a => a.Name == groupedAlbum.Key);
            if (existingArtist is not null)
            {
                foreach (var album in groupedAlbum.Value) 
                    album.CreatedArtist = existingArtist;
                
                existingArtist.Albums.AddRange(groupedAlbum.Value);
            }
            else
                library.Artists.Add(artistFactory.Create(groupedAlbum.Key, groupedAlbum.Value));
        }
    }
}