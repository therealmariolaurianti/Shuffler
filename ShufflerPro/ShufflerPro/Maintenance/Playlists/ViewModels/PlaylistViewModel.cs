using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using ShufflerPro.Core.Objects;
using ShufflerPro.Core.Task;

namespace ShufflerPro.Maintenance.Playlists.ViewModels
{
    public class PlaylistViewModel : PropertyChangedBase, IHandle<PlaylistTask>
    {
        private Dictionary<string, List<Song>> _artistsWithSongs = new Dictionary<string, List<Song>>();
        private ObservableCollection<PlayList> _playlists = new ObservableCollection<PlayList>();

        public PlaylistViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
        }

        public void Handle(PlaylistTask playlist)
        {
            _artistsWithSongs = playlist.Songs
                .GroupBy(song => song.Artist)
                .ToDictionary(s => s.Key, g => g.ToList());

            CreatePlaylists();
        }

        private void CreatePlaylists()
        {
            foreach (var artist in _artistsWithSongs)
            {
                var songs = _artistsWithSongs[artist.Key].ToList();
                var playlist = new PlayList(artist.Key, songs);
                Playlists.Add(playlist);
            }
        }

        public ObservableCollection<PlayList> Playlists 
        {
            get { return _playlists; }
            set
            {
                if (Equals(value, _playlists)) return;
                _playlists = value;
                NotifyOfPropertyChange();
            }
        }
    }
}