using System.Collections.Generic;
using ShufflerPro.Core.Objects;

namespace ShufflerPro.Core.Tasks
{
    public class PlaylistTask
    {
        public List<Song> Songs { get; }

        public PlaylistTask(IEnumerable<Song> songs)
        {
            Songs = new List<Song>(songs);
        }
    }
}