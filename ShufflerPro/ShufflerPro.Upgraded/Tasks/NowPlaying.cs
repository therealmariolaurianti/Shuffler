using ShufflerPro.Upgraded.Objects;

namespace ShufflerPro.Upgraded.Tasks
{
    public class NowPlaying
    {
        public NowPlaying(Song song)
        {
            Song = song;
        }

        public Song Song { get; }
    }
}