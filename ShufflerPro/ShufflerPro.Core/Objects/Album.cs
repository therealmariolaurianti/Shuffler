using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShufflerPro.Core.Objects
{
    public class AlbumFactory
    {
        public Album Create(string artist, string name, List<Song> songs)
        {
            return new Album(artist, name, songs);
        }
    }

    [DebuggerDisplay("{Name}")]
    public class Album
    {
        public Album(string artist, string name, List<Song> songs)
        {
            Artist = artist;
            Name = name;
            Songs = songs.OrderBy(s => s.Track).ToList();
        }

        public string Artist { get; }
        public string Name { get; }
        public List<Song> Songs { get; }
    }
}