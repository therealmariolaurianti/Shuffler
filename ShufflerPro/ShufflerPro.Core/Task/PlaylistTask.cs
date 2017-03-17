using System.Collections.Generic;
using ShufflerPro.Core.Objects;

namespace ShufflerPro.Core.Task
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