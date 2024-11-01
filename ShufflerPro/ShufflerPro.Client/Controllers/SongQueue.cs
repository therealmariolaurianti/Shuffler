using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Controllers;

public class SongQueue
{
    public Song? PreviousSong { get; set; }
    public Song? CurrentSong { get; set; }
    public Song? NextSong { get; set; }
}