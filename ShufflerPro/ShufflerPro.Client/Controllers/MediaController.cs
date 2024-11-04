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
        var songsPaths = Path.GetFullPath(mediaLibraryPath)
            .GetFilesByExtension()
            .ToHashSet();

        return songsPaths.Select(SongFactory.Create).ToList();
    }

    private void Process(Library library, List<Song> songs)
    {
        var songDictionary = songs.GroupBy(s => new
        {
            s.Artist,
            s.Album
        }).ToDictionary(s => s.Key, s => s.ToList());

        foreach (var keyValue in songDictionary)
        {
            var artistKey = keyValue.Key.Artist;
            var artist = library.Artists.SingleOrDefault(a => a.Name == artistKey);
            if (artist is not null)
            {
                var album = albumFactory.Create(artist,
                    keyValue.Key.Album,
                    keyValue.Value.OrderBy(v => v.Track).ToList());
                artist.Albums.Add(album);
            }
            else
            {
                var createdArtist = artistFactory.Create(artistKey, []);
                var album = albumFactory.Create(createdArtist,
                    keyValue.Key.Album,
                    keyValue.Value.OrderBy(v => v.Track).ToList());

                createdArtist.Albums.Add(album);
                library.Artists.Add(createdArtist);
            }
        }
    }
}