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
using ShufflerPro.Maintenance.Playlists.ViewModels;

namespace ShufflerPro.Maintenance.Shell.ViewModels
{
    public class ShellViewModel : PropertyChangedBase, IHandle<NowPlaying>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISongFactory _songFactory;
        private Player _player;
        private readonly IPlayerFactory _playerFactory;

        public ShellViewModel(ISongFactory songFactory, Player player,
            IEventAggregator eventAggregator, IPlayerFactory playerFactory)
        {
            eventAggregator.Subscribe(this);
            _songFactory = songFactory;
            _player = player;
            _eventAggregator = eventAggregator;
            _playerFactory = playerFactory;
            PlaylistViewModel = new PlaylistViewModel(_eventAggregator);
        }

        public bool CanPause => false;
        public bool CanPlay => true;
        public bool CanPrevious => false;
        public bool CanSkip => true;
        public bool CanStop => true;

        public string FolderPath { get; set; }
        public PlayingViewModel PlayingViewModel { get; set; }
        public PlaylistViewModel PlaylistViewModel { get; set; }

        public Queue<Song> Songs => FolderPath
            .GetFilesByExtenstion("mp3")
            .Select(file => _songFactory.Create(file))
            .DistinctBy(s => s.Title)
            .Shuffle()
            .ToQueue();

        public void Handle(NowPlaying nowPlaying)
        {
            PlayingViewModel = new PlayingViewModel(nowPlaying.Song);
            NotifyOfPropertyChange(nameof(PlayingViewModel));
        }

        public void Play()
        {
            if (string.IsNullOrEmpty(FolderPath))
                return;

            var songs = Songs;
            _eventAggregator.PublishOnUIThreadAsync(new PlaylistTask(songs));

            Task.Run(() =>
            {
                if(_player == null)
                    _player = _playerFactory.Create(songs);

                if (_player.Playing)
                {
                    Stop();
                    _player = _playerFactory.Create(songs);
                }

                _player.Songs = songs;
                _player.Play();
            });

            NotifyOfPropertyChange(nameof(CanSkip));
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Stop()
        {
            _player.Stop();
            _player.Dispose();
            _player = null;
        }

        public void Skip()
        {
            _player.Skip();
        }

        public void Previous()
        {
        }
    }
}