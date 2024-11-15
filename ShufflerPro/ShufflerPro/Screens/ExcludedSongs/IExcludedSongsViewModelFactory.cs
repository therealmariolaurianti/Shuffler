using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Screens.ExcludedSongs;

public interface IExcludedSongsViewModelFactory : IFactory
{
    ExcludedSongsViewModel Create(Library library);
}