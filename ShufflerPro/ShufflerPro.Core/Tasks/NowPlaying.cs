using ShufflerPro.Core.Objects;

namespace ShufflerPro.Core.Tasks
{
    public class NowPlaying
    {
        public Song Song { get; }

        public NowPlaying(Song song)
        {
            Song = song;
        }
    }
}