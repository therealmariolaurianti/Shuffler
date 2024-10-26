using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShufflerPro.Core.Objects
{
    [DebuggerDisplay("{Name}")]
    public class Artist
    {
        public Artist(string name, List<Album> albums)
        {
            Name = name;
            Albums = albums.OrderBy(a => a.Name).ToList();
        }

        public string Name { get; }
        public List<Album> Albums { get; }
    }

    public class ArtistFactory
    {
        public Artist Create(string name, List<Album> albums)
        {
            return new Artist(name, albums);
        }
    }
}