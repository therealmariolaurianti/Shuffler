using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Helpers.Extensions;
using NAudio.Wave;
using TagLib;

namespace ShufflerPro.Core.Objects
{
    [DebuggerDisplay("{Id}, {Artist}, {Title}")]
    public class Song
    {
        protected bool Equals(Song other)
        {
            return string.Equals(Title, other.Title);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Song) obj);
        }

        public override int GetHashCode()
        {
            return (Title != null ? Title.GetHashCode() : 0);
        }

        public Song(string path)
        {
            Path = path;

            var songFile = File.Create(Path);
            Title = songFile.Tag.Title;
            Track = (int) songFile.Tag.Track;
            Artist = songFile.Tag.FirstAlbumArtist.ToTitleCase();
            Album = songFile.Tag.Album;
            Genre = songFile.Tag.FirstGenre;

            Debug.WriteLine(Id);
        }

        public string Genre { get; }
        public int Id => NextId.GetNext();
        public string Title { get; }
        public int Track { get; }
        public string Artist { get; }
        public string Album { get; }
        public string Path { get; set; }
        public string Time { get; }
    }
}