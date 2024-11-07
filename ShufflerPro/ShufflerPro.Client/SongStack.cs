using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;

namespace ShufflerPro.Client;

public class SongStack
{
    public List<Song> Stack { get; set; } = [];

    public Song? GetPrevious(Song? currentSong)
    {
        if (currentSong is null)
            return null;

        var index = Stack.IndexOf(currentSong) - 1;
        return Stack.TryGetIndex(index);
    }
}