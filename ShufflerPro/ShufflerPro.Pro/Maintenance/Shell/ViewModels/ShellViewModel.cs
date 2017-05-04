using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Helpers.Extensions;
using ShufflerPro.Core.Objects;
using ShufflerPro.Core.Workers;
using ShufflerPro.Loader;

namespace ShufflerPro.Pro.Maintenance.Shell.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly Runner _runner;
        private Artist _selectedArtist;
        private ObservableCollection<Song> _songs;
        private Album _selectedAlbum;
        private ObservableCollection<Album> _albums;
        private Song _selectedSong;
        private readonly Player _player;

        public ShellViewModel(Runner runner, Player player)
        {
            _runner = runner;
            _player = player;
        }

        protected override void OnInitialize()
        {
            DisplayName = "Shuffler Pro";

            Artists = _runner.Artists;

            Songs = AllSongs.ToObservableCollection();
            Albums = AllAlbums.ToObservableCollection();

            base.OnInitialize();
        }

        
        public void PlaySong(bool doubleClicked)
        {
            Task.Run(async () =>
            {
                if (_player.Playing)
                    _player.Cancel();

                await _player.PlaySong(CurrentSong);
                
            }).ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
            {
                //TODO cant get double click and continuous play to work in sync
                //TODO can only get one or the other
                //if(!doubleClicked)
                //{
                //    SetCurrentSong();
                //}
                //
                //PlaySong(!_player.CompletedSong);
            });
        }

        public Song CurrentSong { get; set; }

        private void SetCurrentSong()
        {
            var song = Songs.Single(s => s.Title == CurrentSong.Title);
            var songIndex = Songs.IndexOf(song);
            var nextSong = Songs[songIndex + 1];
            CurrentSong = nextSong;
        }

        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set
            {
                if (Equals(value, _songs)) return;
                _songs = value;
                NotifyOfPropertyChange();
            }
        }

        
        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set
            {
                if (Equals(value, _albums)) return;
                _albums = value;
                NotifyOfPropertyChange();
            }
        }

        public static List<Artist> Artists { get; set; }
        private static IEnumerable<Song> AllSongs => AllAlbums.SelectMany(album => album.Songs);
        private static IEnumerable<Album> AllAlbums => Artists.SelectMany(artist => artist.Albums);

        public Artist SelectedArtist
        {
            get => _selectedArtist;
            set
            {
                if (Equals(value, _selectedArtist)) return;
                _selectedArtist = value;
                NotifyOfPropertyChange();
                FilterAlbums(value?.Name);
                FilterSongs(value?.Name);
            }
        }

        private void FilterAlbums(string artist)
        {
            Albums = artist == null 
                ? AllAlbums.ToObservableCollection() 
                : AllAlbums.Where(a => a.Artist == artist).ToObservableCollection();
        }

        public Album SelectedAlbum
        {
            get { return _selectedAlbum; }
            set
            {
                if (Equals(value, _selectedAlbum)) return;
                _selectedAlbum = value;
                NotifyOfPropertyChange();
                FilterSongs(SelectedArtist?.Name, value?.Name);
            }
        }

        public Song SelectedSong
        {
            get => _selectedSong;
            set
            {
                if (Equals(value, _selectedSong)) return;
                _selectedSong = value;
                NotifyOfPropertyChange();
                CurrentSong = value;
            }
        }

        private void FilterSongs(string artist, string album = null)
        {
            if (artist == null && album == null)
                Songs = AllSongs.ToObservableCollection();
            if (artist != null && album == null)
                Songs = AllSongs.Where(s => s.Artist == artist).ToObservableCollection();
            if (artist == null && album != null)
                Songs = AllSongs.Where(s => s.Album == album).ToObservableCollection();
            if (artist != null && album != null)
                Songs = AllSongs.Where(s => s.Artist == artist && s.Album == album).ToObservableCollection();
        }
    }
}