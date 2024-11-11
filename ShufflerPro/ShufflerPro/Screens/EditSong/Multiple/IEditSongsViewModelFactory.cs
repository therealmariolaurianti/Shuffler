using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Screens.EditSong.Multiple;

public interface IEditSongsViewModelFactory : IFactory
{
    EditSongsViewModel Create(List<Song> songs, Library library);
}