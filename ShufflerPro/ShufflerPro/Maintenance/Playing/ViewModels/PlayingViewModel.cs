using ShufflerPro.Core.Objects;

namespace ShufflerPro.Maintenance.Playing.ViewModels
{
    public class PlayingViewModel
    {
        public PlayingViewModel(Song song)
        {
            Song = song;
        }

        public Song Song { get; set; }
    }
}