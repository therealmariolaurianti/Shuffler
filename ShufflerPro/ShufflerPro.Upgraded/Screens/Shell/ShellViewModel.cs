using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Factories;

namespace ShufflerPro.Upgraded.Screens.Shell;

public class ShellViewModel : ViewModelBase
{
    private readonly BinaryHelper _binaryHelper;
    private readonly LibraryFactory _libraryFactory;
    private readonly MediaController _mediaController;
    private readonly PlayerController _playerController;
    private readonly SourceFolderController _sourceFolderController;
    private int _applicationVolumeLevel;
    private Song? _currentSong;
    private double _elapsedRunningTime;
    private string _elapsedRunningTimeDisplay;
    private Library _library;
    private Album? _selectedAlbum;
    private Artist? _selectedArtist;
    private Song? _selectedSong;
    private ObservableCollection<Song>? _songs;
    private ObservableCollection<SourceFolder> _sourceFolders;
    private ObservableCollection<TreeViewItem> _sourceTreeItems;

    private CountDownTimer? _timer;

    public ShellViewModel(
        PlayerController playerController,
        SourceFolderController sourceFolderController,
        MediaController mediaController,
        LibraryFactory libraryFactory, BinaryHelper binaryHelper)
    {
        _playerController = playerController;
        _sourceFolderController = sourceFolderController;
        _mediaController = mediaController;
        _libraryFactory = libraryFactory;
        _binaryHelper = binaryHelper;

        TimeSpan = new TimeSpan();

        _playerController.SongChanged += OnSongChanged;
    }

    public Song? CurrentSong
    {
        get => _currentSong;
        set
        {
            if (Equals(value, _currentSong)) return;
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


    public ObservableCollection<Album> Albums =>
        SelectedArtist?.Albums.OrderBy(a => a.Name).ToObservableCollection() ??
        AllAlbums.OrderBy(a => a.Name).ToObservableCollection();

    public Library Library
    {
        get => _library;
        private set
        {
            if (Equals(value, _library)) return;
            _library = value;
            NotifyOfPropertyChange();
            NotifyCollectionsChanged();
        }
    }

    public IReadOnlyCollection<Artist> Artists => Library.Artists.OrderBy(a => a.Name).ToReadOnlyCollection();

    private IReadOnlyCollection<Song> AllSongs => Library.Songs;

    private IReadOnlyCollection<Album> AllAlbums => Library.Albums;

    public Artist? SelectedArtist
    {
        get => _selectedArtist;
        set
        {
            if (Equals(value, _selectedArtist)) return;
            _selectedArtist = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(Albums));

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

    public ObservableCollection<TreeViewItem> SourceTreeItems
    {
        get => _sourceTreeItems;
        set
        {
            if (Equals(value, _sourceTreeItems)) return;
            _sourceTreeItems = value;
            NotifyOfPropertyChange();
        }
    }

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

    public bool IsPlaying => _playerController.Playing;

    public BitmapImage? CurrentSongPicture => _binaryHelper.ToImage(CurrentSong?.Picture);

    public int ApplicationVolumeLevel
    {
        get => _applicationVolumeLevel;
        set
        {
            if (value.Equals(_applicationVolumeLevel)) return;
            _applicationVolumeLevel = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange();
        }
    }

    private void WireTimer()
    {
        _timer = new CountDownTimer();

        _timer!.SetTime(CurrentSong!.Duration!.Value);
        _timer.TimeChanged += () =>
        {
            var timeSpan = CurrentSong?.Duration!.Value.Subtract(_timer.TimeLeft);

            ElapsedRunningTime = timeSpan?.TotalSeconds ?? 0;
            ElapsedRunningTimeDisplay = timeSpan?.ToString("mm':'ss") ?? TimeSpan.ToString("mm':'ss");
        };

        _timer!.Start();
    }

    private void NotifyCollectionsChanged()
    {
        NotifyOfPropertyChange(nameof(Library));
        NotifyOfPropertyChange(nameof(SelectedArtist));
        NotifyOfPropertyChange(nameof(LibrarySummary));
        NotifyOfPropertyChange(nameof(Artists));
        NotifyOfPropertyChange(nameof(Albums));
        NotifyOfPropertyChange(nameof(Songs));
        NotifyOfPropertyChange(nameof(AllSongs));
        NotifyOfPropertyChange(nameof(AllAlbums));
        NotifyOfPropertyChange(nameof(LibrarySummary));
    }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        DisplayName = "mTunes";
        Load();
        SourceFolders = [];
        SourceTreeItems = [];
        InitializeApplicationVolume();

        return base.OnInitializeAsync(cancellationToken);
    }

    private void InitializeApplicationVolume()
    {
        WinImport.waveOutGetVolume(IntPtr.Zero, out var currentVolume);

        var calculatedVolume = (ushort)(currentVolume & 0x0000ffff);
        ApplicationVolumeLevel = calculatedVolume / (ushort.MaxValue / 10);
    }

    private void Load()
    {
        var libraryGuid = Guid.NewGuid();

        _mediaController
            .LoadLibrary(libraryGuid)
            .Do(library =>
            {
                library ??= _libraryFactory.Create(libraryGuid);

                Library = library;

                ElapsedRunningTime = 0;
                ElapsedRunningTimeDisplay = TimeSpan.ToString("mm':'ss");
            });
    }

    public void PlayArtist()
    {
        SelectedSong = SelectedArtist?.Albums.FirstOrDefault()?.Songs.FirstOrDefault();
        PlaySong();
    }

    public void PlayAlbum()
    {
        SelectedSong = SelectedAlbum?.Songs.FirstOrDefault();
        PlaySong();
    }

    private void OnSongChanged(Song obj)
    {
        SelectedSong = obj;
        PlaySong();
    }

    public void PlayPause()
    {
        if (_playerController.Playing)
            _timer?.Pause();
        else
            _timer?.Start();

        _playerController.PlayPause();

        NotifyOfPropertyChange(nameof(IsPlaying));
    }

    public void AdjustApplicationVolume()
    {
        var newVolume = ushort.MaxValue / 10 * ApplicationVolumeLevel;
        var newVolumeAllChannels = ((uint)newVolume & 0x0000ffff) | ((uint)newVolume << 16);

        WinImport.waveOutSetVolume(IntPtr.Zero, newVolumeAllChannels);
    }

    public void PlaySong()
    {
        if (SelectedSong is null)
            return;

        if (_playerController.Playing || _playerController.IsPaused)
            _playerController.Cancel();

        CurrentSong = SelectedSong;
        ElapsedRunningTime = 0;

        WireTimer();

        _playerController.PlaySong(CurrentSong, AllSongs.ToList());

        var playingNow = AllSongs.SingleOrDefault(s => s.IsPlaying);
        if (playingNow is not null)
            playingNow.IsPlaying = false;

        CurrentSong.IsPlaying = true;

        NotifyOfPropertyChange(nameof(IsPlaying));
        NotifyOfPropertyChange(nameof(ElapsedRunningTimeDisplay));
        NotifyOfPropertyChange(nameof(ElapsedRunningTime));
        NotifyOfPropertyChange(nameof(CurrentSongPicture));
    }

    private void FilterSongs(string? artist = null, string? album = null)
    {
        var filteredSongs = AllSongs.AsEnumerable();

        if (artist != null && album == null)
            filteredSongs = AllSongs.Where(s => s.Artist == artist);
        if (artist == null && album != null)
            filteredSongs = AllSongs.Where(s => s.Album == album);
        if (artist != null && album != null)
            filteredSongs = AllSongs.Where(s => s.Artist == artist && s.Album == album);

        Songs = filteredSongs
            .OrderBy(s => s.Artist)
            .ThenBy(s => s.Album)
            .ThenBy(s => s.Track)
            .ToObservableCollection();
    }

    public void AddSource()
    {
        using (var fbd = new FolderBrowserDialog())
        {
            var result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                RunAsync(async () =>
                {
                    _sourceFolderController.BuildSourceFolders(fbd.SelectedPath, SourceFolders)
                        .Do(sourceFolders => _mediaController.LoadFromFolderPath(sourceFolders, Library))
                        .Do(sourceFolders =>
                        {
                            SourceFolders = sourceFolders;

                            SourceTreeItems.Clear();
                            foreach (var sourceFolder in sourceFolders)
                                SourceTreeItems.Add(BuildTreeGridItem(sourceFolder));

                            FilterSongs(SelectedArtist?.Name, SelectedAlbum?.Name);

                            NotifyCollectionsChanged();
                        });
                });
        }
    }

    private static TreeViewItem BuildTreeGridItem(SourceFolder sourceFolder)
    {
        var treeItem = new TreeViewItem
        {
            Header = sourceFolder.Header
        };

        foreach (var sourceFolderItem in sourceFolder.Items)
            treeItem.Items.Add(BuildTreeGridItem(sourceFolderItem));

        return treeItem;
    }

    public void Take()
    {
    }

    public void Skip()
    {
    }
}