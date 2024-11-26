using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Interfaces;

public interface ISongQueue
{
    Song? PreviousSong { get; }
    Song? CurrentSong { get; }
    Song? NextSong { get; }
    void Clear();
}