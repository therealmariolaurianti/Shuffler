using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Ninject;
using ShufflerPro.Core.Objects;

namespace ShufflerPro.Loader
{
    public static class Extensions
    {
        public static List<string> GetFilesByExtension(this string path, List<string> extensions)
        {
            return Directory
                .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => extensions.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()))
                .ToList();
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            return new ObservableCollection<T>(items);
        }
    }

    public class Runner
    {
        private string _path = "C:\\Temp";
        public List<Artist> Artists => Run();

        public List<Artist> Run()
        {
            var kernel = new StandardKernel();
            var albumFactory = kernel.Get<AlbumFactory>();
            var artistFactory = kernel.Get<ArtistFactory>();

            var folderPath = Path.GetFullPath(_path);

            var songs = folderPath.GetFilesByExtension(new List<string> { "mp3" });
            var songFiles = songs.Select(file => new Song(file)).ToList();
            var distinctSongs = songFiles.Distinct().ToList();

            var songFilesWithArtists = distinctSongs.Where(s => s.Artist != null).ToList();
            var songFilesWithoutArtists = distinctSongs.Except(songFilesWithArtists).ToList();

            foreach (var songFilesWithoutArtist in songFilesWithoutArtists)
                songFilesWithoutArtist.UpdateArtist("Artist Unknown");

            songFilesWithArtists.AddRange(songFilesWithoutArtists);

            var albums = songFilesWithArtists.AsParallel().GroupBy(song => song.Album)
                .ToDictionary(s => s.Key, g => g.ToList());
            var albumList = new List<Album>();
            foreach (var kvp in albums)
            {
                var artist = kvp.Value.First().Artist;
                var album = albumFactory.Create(artist, kvp.Key, kvp.Value);
                albumList.Add(album);
            }

            var groupedAlbums = albumList.AsParallel().GroupBy(a => a.Artist).ToDictionary(s => s.Key, g => g.ToList());
            return groupedAlbums.Select(album => artistFactory.Create(album.Key, album.Value)).OrderBy(a => a.Name)
                .ToList();
        }
    }
}