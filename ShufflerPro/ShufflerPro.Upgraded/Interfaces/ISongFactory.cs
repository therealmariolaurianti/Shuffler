using ShufflerPro.Upgraded.Objects;

namespace ShufflerPro.Upgraded.Interfaces
{
    public interface ISongFactory : IFactory
    {
        Song Create(string path);
    }
}