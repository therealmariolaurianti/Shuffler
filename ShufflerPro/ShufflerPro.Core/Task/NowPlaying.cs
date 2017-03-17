using ShufflerPro.Core.Objects;

namespace ShufflerPro.Core.Task
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