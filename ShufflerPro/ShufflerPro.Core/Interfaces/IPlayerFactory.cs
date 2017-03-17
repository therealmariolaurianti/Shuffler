using System.Collections.Generic;
using ShufflerPro.Core.Objects;
using ShufflerPro.Core.Workers;

namespace ShufflerPro.Core.Interfaces
{
    public interface IPlayerFactory : IFactory
    {
        Player Create(Queue<Song> songs);
    }
}