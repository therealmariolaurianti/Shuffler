using System.Windows.Media.Imaging;
using TagLib;

namespace ShufflerPro.Framework.WPF;

public class AlbumArtLoader(BinaryHelper binaryHelper)
{
    public BitmapImage? Load(string? currentSongPath)
    {
        if (currentSongPath is null)
            return null;
        try
        {
            var albumArt = File.Create(currentSongPath).Tag.Pictures.FirstOrDefault();
            return binaryHelper.ToImage(albumArt?.Data.Data);
        }
        catch (Exception)
        {
            return null;
        }
    }
}