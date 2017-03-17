using System.Collections.Generic;
using System.Linq;
using Helpers.Extensions;

namespace ShufflerPro.Core.Objects
{
    //camel case because
    public class PlayList
    {
        public PlayList(string artist, IEnumerable<Song> songs)
        {
            Artist = artist;
            Songs = songs.ToQueue();
        }

        public string Artist { get; set; }
        public Queue<Song> Songs { get; set; }
    }
}