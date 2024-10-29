using System.Collections.ObjectModel;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Extensions;
using ShufflerPro.Upgraded.Objects;
using Screen = Caliburn.Micro.Screen;

namespace ShufflerPro.Upgraded.Screens.Shell;

public class ShellViewModel : Screen
{
    private readonly FolderBrowserController _folderBrowserController;
    private readonly MediaController _mediaController;
    private readonly PlayerController _playerController;

    private readonly CountDownTimer _timer;
    private ObservableCollection<Album>? _albums;
    private Song? _currentSong;
    private double _elapsedRunningTime;
    private string _elapsedRunningTimeDisplay;
    private Album? _selectedAlbum;
    private Artist? _selectedArtist;
    private Song? _selectedSong;
    private ObservableCollection<Song>? _songs;
    private ObservableCollection<SourceFolder> _sourceFolders;

    public ShellViewModel(
        PlayerController playerController,
        CountDownTimer timer,
        FolderBrowserController folderBrowserController, MediaController mediaController)
    {
        _playerController = playerController;
        _timer = timer;
        _folderBrowserController = folderBrowserController;
        _mediaController = mediaController;

        TimeSpan = new TimeSpan();
    }

    public Song? CurrentSong
    {
        get => _currentSong;
        set
        {
            if (Nullable.Equals(value, _currentSong)) return;
            _currentSong = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(MaxRunTime));
            NotifyOfPropertyChange(nameof(CurrentSongTime));
        }
    }

    public ObservableCollection<Song>? Songs
    {
        get => _songs;
        set
        {
            if (Equals(value, _songs)) return;
            _songs = value;
            NotifyOfPropertyChange();
        }
    }


    public ObservableCollection<Album>? Albums
    {
        get => _albums;
        set
        {
            if (Equals(value, _albums)) return;
            _albums = value;
            NotifyOfPropertyChange();
        }
    }

    public static Library Library { get; private set; }

    public static IReadOnlyCollection<Artist> Artists => Library.Artists;

    private static IReadOnlyCollection<Song> AllSongs => Library.Songs;

    private static IReadOnlyCollection<Album> AllAlbums => Library.Albums;

    public Artist? SelectedArtist
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

    public Album? SelectedAlbum
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

    public Song? SelectedSong
    {
        get => _selectedSong;
        set
        {
            if (Equals(value, _selectedSong)) return;
            _selectedSong = value;
            NotifyOfPropertyChange();
        }
    }

    public string LibrarySummary => Library.Summary;

    public double MaxRunTime => CurrentSong?.Duration?.TotalSeconds ?? 0;

    public double ElapsedRunningTime
    {
        get => _elapsedRunningTime;
        set
        {
            if (value.Equals(_elapsedRunningTime)) return;
            _elapsedRunningTime = value;
            NotifyOfPropertyChange();
        }
    }

    public string ElapsedRunningTimeDisplay
    {
        get => _elapsedRunningTimeDisplay;
        set
        {
            if (value == _elapsedRunningTimeDisplay) return;
            _elapsedRunningTimeDisplay = value;
            NotifyOfPropertyChange();
        }
    }

    public string CurrentSongTime => CurrentSong?.Time ?? TimeSpan.ToString("mm':'ss");

    private TimeSpan TimeSpan { get; }

    public ObservableCollection<SourceFolder> SourceFolders
    {
        get => _sourceFolders;
        set
        {
            if (Equals(value, _sourceFolders)) return;
            _sourceFolders = value;
            NotifyOfPropertyChange();
        }
    }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        DisplayName = "mTunes";
        Load();
        SourceFolders = [];

        return base.OnInitializeAsync(cancellationToken);
    }

    private void Load()
    {
        var libraryGuid = Guid.NewGuid();

        _mediaController
            .LoadLibrary(libraryGuid)
            .Do(library =>
            {
                Library = library;

                Songs = AllSongs.ToObservableCollection();
                Albums = AllAlbums.ToObservableCollection();

                ElapsedRunningTime = 0;
                ElapsedRunningTimeDisplay = TimeSpan.ToString("mm':'ss");
            });
    }

    public void PlayArtist()
    {
        SelectedSong = SelectedArtist?.Albums.FirstOrDefault().Songs.FirstOrDefault();
        PlaySong();
    }

    public void PlayAlbum()
    {
        SelectedSong = SelectedAlbum?.Songs.FirstOrDefault();
        PlaySong();
    }

    public void PlaySong()
    {
        if (SelectedSong is null)
            return;

        if (_timer.IsRunning)
            _timer.Stop();

        CurrentSong = SelectedSong;
        ElapsedRunningTime = 0;

        if (_playerController.Playing)
            _playerController.Cancel();

        _timer.SetTime(CurrentSong.Value.Duration!.Value);
        _timer.Start();
        _timer.TimeChanged += () =>
        {
            var timeSpan = CurrentSong.Value.Duration!.Value.Subtract(_timer.TimeLeft);

            ElapsedRunningTime = timeSpan.TotalSeconds;
            ElapsedRunningTimeDisplay = timeSpan.ToString("mm':'ss");
        };

        _playerController.PlaySong(CurrentSong.Value);
    }

    private void FilterAlbums(string? artist)
    {
        Albums = artist == null
            ? AllAlbums.ToObservableCollection()
            : AllAlbums.Where(a => a.Artist == artist).ToObservableCollection();
    }

    private void FilterSongs(string? artist, string? album = null)
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

    public void AddSource()
    {
        //_folderBrowserController.Add(SourceFolders);
    }
}