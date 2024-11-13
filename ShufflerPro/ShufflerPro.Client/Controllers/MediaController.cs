using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;
using ShufflerPro.Client.Factories;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class MediaController(
    ArtistFactory artistFactory,
    AlbumFactory albumFactory)
{
    public NewResult<NewUnit> LoadFromFolderPath(ICollection<SourceFolder> sourceFolders,
        Library library, List<ExcludedSong> excludedSongs)
    {
        return NewResultExtensions.Try(() =>
        {
            var roots = sourceFolders.Where(sf => sf.IsRoot);
            foreach (var root in roots)
                ProcessPath(library, root, excludedSongs);

            return NewUnit.Default;
        });
    }

    private void ProcessPath(Library library, SourceFolder sourceFolder, List<ExcludedSong> excludedSongs)
    {
        if (sourceFolder is { IsRoot: false, IsProcessed: false, Items.Count: 0 })
        {
            var path = sourceFolder.FullPath;
            var loadSongsInPath = LoadSongsInPath(path, sourceFolder, excludedSongs);

            Process(library, loadSongsInPath);
        }

        sourceFolder.IsProcessed = true;

        foreach (var folderItem in sourceFolder.Items)
            ProcessPath(library, folderItem, excludedSongs);
    }

    private static List<Song> LoadSongsInPath(string mediaLibraryPath, SourceFolder sourceFolder,
        List<ExcludedSong> excludedSongs)
    {
        try
        {
            var songsPaths = Path.GetFullPath(mediaLibraryPath)
                .GetFilesByExtension()
                .ToHashSet();

            var songs = songsPaths.AsParallel().Select(SongFactory.Create).ToList();
            var songsToExclude = songs.Where(s => excludedSongs.Select(es => es.SongId).Contains(s.Id));

            return songs.Except(songsToExclude).ToList();
        }
        catch (Exception)
        {
            sourceFolder.IsValid = false;
            return [];
        }
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