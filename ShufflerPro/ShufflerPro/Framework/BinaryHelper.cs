using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ShufflerPro.Result;
using TagLib;
using File = System.IO.File;

namespace ShufflerPro.Framework;

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

    public NewResult<byte[]?> Add()
    {
        var fileDialog = new OpenFileDialog();
        return fileDialog.ShowDialog() == true
            ? CreateBinaryImage(fileDialog)
            : NewResultExtensions.CreateFail<byte[]?>("");
    }

    private static byte[] CreateBinaryImage(OpenFileDialog fileDialog)
    {
        Stream stream = File.OpenRead(fileDialog.FileName);
        var binaryImage = new byte[stream.Length];
        _ = stream.Read(binaryImage, 0, (int)stream.Length);
        return binaryImage;
    }
}