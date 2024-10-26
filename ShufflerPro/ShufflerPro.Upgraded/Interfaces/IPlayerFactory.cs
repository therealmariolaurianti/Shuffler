using ShufflerPro.Upgraded.Objects;
using ShufflerPro.Upgraded.Workers;

namespace ShufflerPro.Upgraded.Interfaces
{
    public interface IPlayerFactory : IFactory
    {
        Player Create(Queue<Song> songs);
    }
}