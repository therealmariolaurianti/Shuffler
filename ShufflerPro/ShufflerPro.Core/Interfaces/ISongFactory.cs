using ShufflerPro.Core.Objects;

namespace ShufflerPro.Core.Interfaces
{
    public interface ISongFactory : IFactory
    {
        Song Create(string path);
    }
}