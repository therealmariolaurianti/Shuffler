using System.Collections.Generic;
using ShufflerPro.Core.Objects;

namespace ShufflerPro.Core.Task
{
    public class PlaylistTask
    {
        public Queue<Song> Songs { get; }

        public PlaylistTask(Queue<Song> songs)
        {
            Songs = songs;
        }
    }
}