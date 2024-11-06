using System.Security.Cryptography;
using System.Text;
using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client;

public static class SongExtensions
{
    public static Guid Hash(this Song song, string path)
    {
        var input = $"{path}{song.Title}{song.Artist}{song.Album}{song.Time}";
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        
        return new Guid(hash);
    }
}