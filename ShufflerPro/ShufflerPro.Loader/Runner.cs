using System;
using System.Collections.Generic;
using System.Linq;
using Helpers.Extensions;
using ShufflerPro.Core.Objects;

namespace ShufflerPro.Loader
{
    public class Runner
    {
        public Dictionary<string, Dictionary<string, List<Song>>> SongLibrary => Run();

        public Dictionary<string, Dictionary<string, List<Song>>> Run()
        {
            var folderPath = @"D:\Music Library";
            var songPaths = folderPath.GetFilesByExtenstion("mp3");
            var songFiles = songPaths.Select(file => new Song(file)).ToList();
            var songFilesWithArtists = songFiles.Where(s => s.Artist != null).ToList();
            
            //TODO
            var songFilesWithoutArtists = songFiles.Except(songFilesWithArtists).ToList();

            var filteredArtists = new Dictionary<string, Dictionary<string, List<Song>>>();

            var artists = songFilesWithArtists.GroupBy(song => song.Artist).ToDictionary(s => s.Key, g => g.ToList());
            foreach (var kvp in artists)
            {
                var albums = kvp.Value.GroupBy(song => song.Album)
                    .ToDictionary(s => s.Key, g => g.Distinct().ToList());

                filteredArtists.Add(kvp.Key, albums);
            }

            return filteredArtists;
        }
    }
}