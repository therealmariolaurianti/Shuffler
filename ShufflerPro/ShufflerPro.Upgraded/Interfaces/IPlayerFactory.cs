using ShufflerPro.Upgraded.Controllers;
using ShufflerPro.Upgraded.Objects;

namespace ShufflerPro.Upgraded.Interfaces
{
    public interface IPlayerFactory : IFactory
    {
        PlayerController Create(Queue<Song> songs);
    }
}