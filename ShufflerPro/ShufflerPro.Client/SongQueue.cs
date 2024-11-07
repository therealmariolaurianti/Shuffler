using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Client;

public class SongQueue : ISongQueue
{
    public Song? PreviousSong { get; set; }
    public Song? CurrentSong { get; set; }
    public Song? NextSong { get; set; }
}