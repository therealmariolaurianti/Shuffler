using System.Diagnostics;
using TagLib;

namespace ShufflerPro.Core.Objects
{
    [DebuggerDisplay("{Title}")]
    public class Song
    {
        public Song(string path)
        {
            Path = path;

            var songFile = File.Create(Path);
            Title = songFile.Tag.Title;
            Track = (int)songFile.Tag.Track;
            Artist = songFile.Tag.FirstAlbumArtist;
            Album = songFile.Tag.Album;
            Genre = songFile.Tag.FirstGenre;
            Id = NextId.GetNext();

#if DEBUG
            Debug.WriteLine(Id);
#endif
        }

        public string Genre { get; }
        public int Id { get; }
        public string Title { get; }
        public int Track { get; }
        public string Artist { get; private set; }
        public string Album { get; }
        public string Path { get; set; }
        public string Time { get; }

        protected bool Equals(Song other)
        {
            return string.Equals(Title, other.Title);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Song)obj);
        }

        public override int GetHashCode()
        {
            return Title != null ? Title.GetHashCode() : 0;
        }

        public void UpdateArtist(string artist)
        {
            Artist = artist;
        }
    }
}