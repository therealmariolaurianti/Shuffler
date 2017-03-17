using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Helpers.Extensions;
using ShufflerPro.Core.Interfaces;
using ShufflerPro.Core.Objects;
using ShufflerPro.Core.Task;
using ShufflerPro.Core.Workers;
using ShufflerPro.Maintenance.Playing.ViewModels;
using ShufflerPro.Properties;

namespace ShufflerPro.Maintenance.Shell.ViewModels
{
    public class ShellViewModel : PropertyChangedBase, IHandle<NowPlaying>
    {
        private readonly ISongFactory _songFactory;
        private readonly Player _player;
        private readonly IEventAggregator _eventAggregator;

        public ShellViewModel(ISongFactory songFactory, Player player, 
            IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
            _songFactory = songFactory;
            _player = player;
            _eventAggregator = eventAggregator;
        }

        public string FolderPath => Settings.Default.FolderPath;
        public PlayingViewModel PlayingViewModel { get; set; }

        public Queue<Song> Songs => FolderPath
                .GetFilesByExtenstion("mp3")
                .Select(file => _songFactory.Create(file))
                .Shuffle()
                .ToQueue();

        public void Play()
        {
            var songs = Songs;
            _eventAggregator.PublishOnUIThread(new PlaylistTask(songs));

            Task.Run(() =>
            {
                _player.Songs = songs;
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

        public void Handle(NowPlaying nowPlaying)
        {
            PlayingViewModel = new PlayingViewModel(nowPlaying.Song);
            NotifyOfPropertyChange(nameof(PlayingViewModel));
        }
    }
}