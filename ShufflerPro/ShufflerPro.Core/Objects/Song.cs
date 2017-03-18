using System.Diagnostics;
using TagLib;

namespace ShufflerPro.Core.Objects
{
    [DebuggerDisplay("{Id}, {Artist}, {Title}")]
    public class Song
    {
        private readonly int _id = 0;

        public Song(string path)
        {
            Path = path;

            Id = _id.GetNext();
            Debug.WriteLine(Id);
        }

        public int Id { get; }
        public Tag SongFile => File.Create(Path).Tag;
        public string Title => SongFile.Title;
        public string Track => SongFile.Track.ToString();
        public string Artist => SongFile.FirstAlbumArtist;
        public string Path { get; set; }
    }
}