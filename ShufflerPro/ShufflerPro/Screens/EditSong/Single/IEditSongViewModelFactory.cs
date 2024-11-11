using System.Windows.Media.Imaging;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Screens.EditSong.Single;

public interface IEditSongViewModelFactory : IFactory
{
    EditSongViewModel Create(Song song, BitmapImage? albumArt);
}