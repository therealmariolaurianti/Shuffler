using System.IO;
using System.Windows.Media.Imaging;
using TagLib;

namespace ShufflerPro.Upgraded.Framework;

public class BinaryHelper
{
    public BitmapImage? ToImage(IPicture? picture)
    {
        try
        {
            var array = picture?.Data.Data;
            if (array is null || array.Length == 0)
                return null;

            using var ms = new MemoryStream(array);
            var image = new BitmapImage();

            image.BeginInit();

            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;

            image.EndInit();

            return image;
        }
        catch (Exception)
        {
            return null;
        }
    }
}