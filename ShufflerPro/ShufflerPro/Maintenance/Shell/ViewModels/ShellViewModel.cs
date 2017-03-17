using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Helpers.Extensions;
using ShufflerPro.Core.Interfaces;
using ShufflerPro.Core.Objects;
using ShufflerPro.Core.Workers;

namespace ShufflerPro.Maintenance.Shell.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private readonly IPlayerFactory _playerFactory;
        private readonly ISongFactory _songFactory;

        public ShellViewModel(IPlayerFactory playerFactory,
            ISongFactory songFactory)
        {
            _playerFactory = playerFactory;
            _songFactory = songFactory;
        }

        public string FolderPath { get; set; }

        public Player Player => _playerFactory.Create(Songs);

        public Queue<Song> Songs => FolderPath
                .GetFilesByExtenstion("mp3")
                .Select(file => _songFactory.Create(file))
                .Shuffle()
                .ToQueue();

        public void Play()
        {
            Player.Play();
        }

        public void Pause()
        {
            Player.Pause();
        }

        public void Stop()
        {
            Player.Stop();
        }
    }
}