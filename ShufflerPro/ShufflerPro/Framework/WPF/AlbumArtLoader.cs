using System.Windows.Media.Imaging;
using TagLib;

namespace ShufflerPro.Framework.WPF;

public class AlbumArtLoader
{
    private readonly BinaryHelper _binaryHelper;

    public AlbumArtLoader(BinaryHelper binaryHelper)
    {
        _binaryHelper = binaryHelper;
    }

    public BitmapImage? Load(string? currentSongPath)
    {
        if (currentSongPath is null)
            return null;
        var albumArt = File.Create(currentSongPath).Tag.Pictures.FirstOrDefault();
        return _binaryHelper.ToImage(albumArt);
    }
}