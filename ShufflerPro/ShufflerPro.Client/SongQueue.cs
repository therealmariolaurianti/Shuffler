using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client;

public class SongQueue : ISongQueue
{
    public Song? PreviousSong { get; set; }
    public Song? CurrentSong { get; set; }
    public Song? NextSong { get; set; }
}

public interface ISongQueue
{
    Song? PreviousSong { get; }
    Song? CurrentSong { get; }
    Song? NextSong { get; }
}