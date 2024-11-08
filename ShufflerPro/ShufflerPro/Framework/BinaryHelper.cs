using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ShufflerPro.Result;
using TagLib;
using File = System.IO.File;

namespace ShufflerPro.Framework;

public class BinaryHelper
{
    public BitmapImage? ToImage(byte[]? data)
    {
        try
        {
            if (data is null || data.Length == 0)
                return null;
            
            using var ms = new MemoryStream(data);
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

    public byte[]? ToBytes(BitmapImage? bitmapImage)
    {
        if (bitmapImage is null)
            return null;

        byte[] data;
        var encoder = new JpegBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
        using (var ms = new MemoryStream())
        {
            encoder.Save(ms);
            data = ms.ToArray();
        }

        return data;
    }
}