using System.Collections.Generic;

namespace ShufflerPro.Core.Objects
{
    public class PlayList
    {
        public PlayList(string artist, IEnumerable<Song> songs)
        {
            Artist = artist;
            Songs = new Queue<Song>(songs);
        }

        public string Artist { get; set; }
        public Queue<Song> Songs { get; set; }
    }
}