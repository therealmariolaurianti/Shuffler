using ShufflerPro.Core.Objects;

namespace ShufflerPro.Core.Tasks
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