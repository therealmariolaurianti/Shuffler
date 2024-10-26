using System.Collections.Generic;
using ShufflerPro.Core.Objects;

namespace ShufflerPro.Core.Tasks
{
    public class PlaylistTask
    {
        public PlaylistTask(IEnumerable<Song> songs)
        {
            Songs = new List<Song>(songs);
        }

        public List<Song> Songs { get; }
    }
}