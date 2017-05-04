using System.Collections.Generic;
using System.Linq;
using Helpers.Extensions;
using Ninject;
using ShufflerPro.Core.Objects;

namespace ShufflerPro.Loader
{
    public class Runner
    {
        public List<Artist> Artists => Run();

        public List<Artist> Run()
        {
            var kernel = new StandardKernel();
            var albumFactory = kernel.Get<AlbumFactory>();
            var artistFactory = kernel.Get<ArtistFactory>();

            var folderPath = @"D:\Music Library";

            var songPaths = folderPath.GetFilesByExtenstion("mp3").Take(500);
            var songFiles = songPaths.Select(file => new Song(file)).ToList();
            var distinctSongs = songFiles.Distinct().ToList();

            var songFilesWithArtists = distinctSongs.Where(s => s.Artist != null).ToList();
            var songFilesWithoutArtists = distinctSongs.Except(songFilesWithArtists).ToList();

            foreach (var songFilesWithoutArtist in songFilesWithoutArtists)
                songFilesWithoutArtist.UpdateArtist("Artist Unknown");

            songFilesWithArtists.AddRange(songFilesWithoutArtists);

            var albums = songFilesWithArtists.AsParallel().GroupBy(song => song.Album).ToDictionary(s => s.Key, g => g.ToList());
            var albumList = new List<Album>();
            foreach (var kvp in albums)
            {
                var artist = kvp.Value.First().Artist;
                var album = albumFactory.Create(artist, kvp.Key, kvp.Value);
                albumList.Add(album);
            }

            var groupedAlbums = albumList.AsParallel().GroupBy(a => a.Artist).ToDictionary(s => s.Key, g => g.ToList());
            return groupedAlbums.Select(album => artistFactory.Create(album.Key, album.Value)).OrderBy(a => a.Name).ToList();
        }
    }
}