using ShufflerPro.Client.Entities;

namespace ShufflerPro.Client.Interfaces
{
    public interface ISongFactory : IFactory
    {
        Song Create(string path);
    }
}