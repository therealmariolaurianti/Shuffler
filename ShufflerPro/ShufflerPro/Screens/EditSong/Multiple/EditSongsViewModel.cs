using System.Windows.Media.Imaging;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Framework.WPF;

namespace ShufflerPro.Screens.EditSong.Multiple;

public interface IEditSongsViewModelFactory : IFactory
{
    EditSongsViewModel Create(List<Song> songs);
}

public class EditSongsViewModel : ViewModelBase
{
    
}