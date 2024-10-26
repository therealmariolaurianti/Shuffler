using ShufflerPro.Upgraded.Objects;

namespace ShufflerPro.Upgraded.Tasks
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