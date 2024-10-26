using System.Collections.ObjectModel;
using Caliburn.Micro;
using ShufflerPro.Upgraded.Objects;
using ShufflerPro.Upgraded.Workers;

namespace ShufflerPro.Upgraded;

public class ShellViewModel : Screen
{
    private readonly Player _player;
    private readonly Runner _runner;
    private ObservableCollection<Album> _albums;
    private Album _selectedAlbum;
    private Artist _selectedArtist;
    private Song _selectedSong;
    private ObservableCollection<Song> _songs;

    public ShellViewModel(Runner runner, Player player)
    {
        _runner = runner;
        _player = player;
    }

    public Song CurrentSong { get; set; }

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

    public Album SelectedAlbum
    {
        get => _selectedAlbum;
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

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        DisplayName = "Shuffler Pro";

        Artists = _runner.Artists;

        Songs = AllSongs.ToObservableCollection();
        Albums = AllAlbums.ToObservableCollection();

        return base.OnInitializeAsync(cancellationToken);
    }


    public void PlaySong()
    {
        Task.Run(async () =>
        {
            if (_player.Playing)
                _player.Cancel();

            await _player.PlaySong(CurrentSong);
        }).ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
        {
            if (!_player.IsCompleted)
            {
                CurrentSong = SelectedSong;
                return;
            }

            _player.ReInitialize();
            SetNextSong();
            if (CurrentSong != null)
                PlaySong();
        });
    }

    private void SetNextSong()
    {
        try
        {
            var song = Songs.Single(s => s.Track == CurrentSong.Track);
            var songIndex = Songs.IndexOf(song);
            var nextSong = Songs[songIndex + 1];

            CurrentSong = nextSong;
        }
        catch (Exception ex)
        {
            //TODO log exception
            CurrentSong = null;
        }
    }

    private void FilterAlbums(string artist)
    {
        Albums = artist == null
            ? AllAlbums.ToObservableCollection()
            : AllAlbums.Where(a => a.Artist == artist).ToObservableCollection();
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