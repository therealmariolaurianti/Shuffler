using System.Collections.ObjectModel;
using Caliburn.Micro;
using ShufflerPro.Upgraded.Bootstrapper;
using ShufflerPro.Upgraded.Controllers;
using ShufflerPro.Upgraded.Objects;

namespace ShufflerPro.Upgraded.Screens.Shell;

public class ShellViewModel : Screen
{
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

    public ShellViewModel(PlayerController playerController, MediaController mediaController, CountDownTimer timer)
    {
        _playerController = playerController;
        _mediaController = mediaController;
        _timer = timer;

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

    public static IReadOnlyCollection<Artist>? Artists { get; set; }

    private static IReadOnlyCollection<Song>? AllSongs =>
        AllAlbums?.SelectMany(album => album.Songs).ToReadOnlyCollection();

    private static IReadOnlyCollection<Album>? AllAlbums =>
        Artists?.SelectMany(artist => artist.Albums).ToReadOnlyCollection();

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

    public string LibrarySummary
    {
        get
        {
            var totalSongs = AllSongs?.Count ?? 0;
            var totalTime = TimeSpan.FromTicks(AllSongs?.Sum(s => s.Duration?.Ticks) ?? 0);

            return $"{totalSongs} songs, {totalTime:mm':'ss} total time";
        }
    }

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

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        DisplayName = "mTunes";
        Load();

        return base.OnInitializeAsync(cancellationToken);
    }

    private void Load()
    {
        Artists = _mediaController.LoadArtists();

        Songs = AllSongs?.ToObservableCollection();
        Albums = AllAlbums?.ToObservableCollection();

        ElapsedRunningTime = 0;
        ElapsedRunningTimeDisplay = TimeSpan.ToString("mm':'ss");
    }

    public void PlaySong()
    {
        if (SelectedSong is null)
            return;

        if (_timer.IsRunning) _timer.Stop();

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
            ? AllAlbums?.ToObservableCollection()
            : AllAlbums?.Where(a => a.Artist == artist).ToObservableCollection();
    }

    private void FilterSongs(string? artist, string? album = null)
    {
        if (artist == null && album == null)
            Songs = AllSongs?.ToObservableCollection();
        if (artist != null && album == null)
            Songs = AllSongs?.Where(s => s.Artist == artist).ToObservableCollection();
        if (artist == null && album != null)
            Songs = AllSongs?.Where(s => s.Album == album).ToObservableCollection();
        if (artist != null && album != null)
            Songs = AllSongs?.Where(s => s.Artist == artist && s.Album == album).ToObservableCollection();
    }
}