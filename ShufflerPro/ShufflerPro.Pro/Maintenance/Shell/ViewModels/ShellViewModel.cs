using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Helpers.Extensions;
using ShufflerPro.Core.Objects;
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

        public ShellViewModel(Runner runner)
        {
            _runner = runner;
        }

        protected override void OnInitialize()
        {
            DisplayName = "Shuffler Pro";
            SongLibrary = _runner.SongLibrary;

            Songs = AllSongs.ToObservableCollection();
            Albums = AllAlbums.ToObservableCollection();

            base.OnInitialize();
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

        private IEnumerable<Song> AllSongs
        {
            get
            {
                var songs = new List<Song>();
                foreach (var songLibraryValue in SongLibrary.Values)
                {
                    foreach (var loadedSongs in songLibraryValue.Values)
                    {
                        songs.AddRange(loadedSongs);
                    }
                }
                return songs;
            }
        }

        public List<Artist> Artists => SongLibrary.Keys.Select(key => new Artist(key)).OrderBy(a => a.Name).ToList();

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

        private IEnumerable<Album> AllAlbums
        {
            get
            {
                var albums = new List<Album>();
                foreach (var song in SongLibrary.Values)
                {
                    var artist = song.Select(x => x.Value.First().Artist).First();
                    albums.AddRange(song.Select(s => new Album(s.Key, artist)));
                }

                return albums.OrderBy(a => a.Name);
            }
        }

        public Dictionary<string,Dictionary<string,List<Song>>> SongLibrary { get; set; }

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
                FilterSongs(SelectedArtist.Name, value?.Name);
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

    public class Album
    {
        public string Name { get; }
        public string Artist { get; set; }

        public Album(string name, string artist)
        {
            Name = name;
            Artist = artist;
        }
    }

    public class Artist
    {
        public string Name { get; }

        public Artist(string name)
        {
            Name = name;
        }
    }
}