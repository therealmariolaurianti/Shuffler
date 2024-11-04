using TagLib;

namespace ShufflerPro.Upgraded.Framework.WPF;

public class AlbumArtLoader
{
    public IPicture? Load(string? currentSongPath)
    {
        if (currentSongPath is null)
            return null;
        var albumArt = File.Create(currentSongPath).Tag.Pictures.FirstOrDefault();
        return albumArt;
    }
}