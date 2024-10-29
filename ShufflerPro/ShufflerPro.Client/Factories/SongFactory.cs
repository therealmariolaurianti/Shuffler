using ShufflerPro.Client.Entities;
using File = TagLib.File;

namespace ShufflerPro.Client.Factories;

public static class SongFactory
{
    public static Song Create(string songPath)
    {
        try
        {
            var songFile = File.Create(songPath);
            return new Song(songFile, songPath);
        }
        catch (Exception)
        {
            return new Song(null, songPath);
        }
    }
}