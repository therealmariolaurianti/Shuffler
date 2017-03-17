using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Helpers.Extensions;
using ShufflerPro.Core.Interfaces;
using ShufflerPro.Core.Objects;
using ShufflerPro.Core.Workers;
using ShufflerPro.Properties;

namespace ShufflerPro.Maintenance.Shell.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private readonly ISongFactory _songFactory;
        private readonly Player _player;

        public ShellViewModel(ISongFactory songFactory, Player player)
        {
            _songFactory = songFactory;
            _player = player;
        }

        public string FolderPath => Settings.Default.FolderPath;

        public Queue<Song> Songs => FolderPath
                .GetFilesByExtenstion("mp3")
                .Select(file => _songFactory.Create(file))
                .Shuffle()
                .ToQueue();

        public void Play()
        {
            Task.Run(() =>
            {
                _player.Songs = Songs;
                _player.Play();
            });
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Stop()
        {
            _player.Stop();
        }
    }
}