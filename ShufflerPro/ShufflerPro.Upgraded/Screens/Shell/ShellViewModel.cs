using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Enums;
using ShufflerPro.Result;
using ShufflerPro.Upgraded.Framework;
using ShufflerPro.Upgraded.Framework.WPF;
using MessageBox = System.Windows.MessageBox;

namespace ShufflerPro.Upgraded.Screens.Shell;

public class ShellViewModel : ViewModelBase
{
    private readonly BinaryHelper _binaryHelper;

    private readonly ContextMenuBuilder _contextMenuBuilder;

    private readonly LibraryController _libraryController;
    private readonly MediaController _mediaController;
    private readonly PlayerController _playerController;
    private readonly SourceFolderController _sourceFolderController;
    private int _applicationVolumeLevel;
    private Song? _currentSong;
    private double _elapsedRunningTime;
    private string _elapsedRunningTimeDisplay;
    private bool _isLoadingSourceFolders;
    private Library? _library;
    private LibrarySearchType _librarySearchType;
    private string _searchText;
    private Album? _selectedAlbum;
    private Artist? _selectedArtist;
    private Song? _selectedSong;
    private SourceTreeViewItem? _selectedTreeViewItem;
    private ObservableCollection<Song>? _songs;
    private ObservableCollection<SourceTreeViewItem> _sourceTreeItems;

    private CountDownTimer? _timer;

    public ShellViewModel(
        PlayerController playerController,
        SourceFolderController sourceFolderController,
        MediaController mediaController,
        BinaryHelper binaryHelper, LibraryController libraryController, ContextMenuBuilder contextMenuBuilder)
    {
        _playerController = playerController;
        _sourceFolderController = sourceFolderController;
        _mediaController = mediaController;
        _binaryHelper = binaryHelper;
        _libraryController = libraryController;
        _contextMenuBuilder = contextMenuBuilder;

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

    public string SyncText => IsLoadingSourceFolders ? "Syncing" : "Synced";

    public ObservableCollection<Album> Albums =>
        SelectedArtist?.Albums.OrderBy(a => a.Name).ToObservableCollection() ??
        AllAlbums.OrderBy(a => a.Name).ToObservableCollection();

    public Library? Library
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

    public IReadOnlyCollection<Artist> Artists => Library?.Artists.OrderBy(a => a.Name).ToReadOnlyCollection()
                                                  ?? new ReadOnlyCollection<Artist>(new List<Artist>());

    private IReadOnlyCollection<Song> AllSongs => Library?.Songs ?? new ReadOnlyCollection<Song>(new List<Song>());

    private IReadOnlyCollection<Album> AllAlbums => Library?.Albums ?? new ReadOnlyCollection<Album>(new List<Album>());

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

    public string LibrarySummary => Library?.Summary ?? string.Empty;

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

    public ObservableCollection<SourceTreeViewItem> SourceTreeItems
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
        get => Library?.SourceFolders ?? new ObservableCollection<SourceFolder>();
        set
        {
            if (Equals(value, Library!.SourceFolders)) return;
            Library!.SourceFolders = value;
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

    public bool IsLoadingSourceFolders
    {
        get => _isLoadingSourceFolders;
        set
        {
            if (value == _isLoadingSourceFolders) return;
            _isLoadingSourceFolders = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(SyncText));
        }
    }

    private List<Song> AllSongsOrdered => AllSongs.OrderBy(s => s.Artist)
        .ThenBy(s => s.Album)
        .ThenBy(s => s.Track).ToList();

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (value == _searchText) return;
            _searchText = value;
            NotifyOfPropertyChange();
            OnSearchTextChanged();
        }
    }

    public LibrarySearchType LibrarySearchType
    {
        get => _librarySearchType;
        set
        {
            if (value == _librarySearchType) return;
            _librarySearchType = value;
            NotifyOfPropertyChange();
        }
    }

    private void OnSearchTextChanged()
    {
        switch (LibrarySearchType)
        {
            case LibrarySearchType.Artist:
                SearchSongs(SearchText, null, null);
                break;
            case LibrarySearchType.Song:
                SearchSongs(null, null, SearchText);
                break;
            case LibrarySearchType.Album:
                SearchSongs(null, SearchText, null);
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
        NotifyOfPropertyChange(nameof(Artists));
        NotifyOfPropertyChange(nameof(Albums));
        NotifyOfPropertyChange(nameof(Songs));
        NotifyOfPropertyChange(nameof(AllSongs));
        NotifyOfPropertyChange(nameof(AllAlbums));
        NotifyOfPropertyChange(nameof(LibrarySummary));
        NotifyOfPropertyChange(nameof(SourceFolders));
    }

    protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        DisplayName = "mTunes";
        SourceTreeItems = [];
        LibrarySearchType = LibrarySearchType.Artist;
        InitializeApplicationVolume();

        await Load();
    }

    private void InitializeApplicationVolume()
    {
        WinImport.waveOutGetVolume(IntPtr.Zero, out var currentVolume);

        var calculatedVolume = (ushort)(currentVolume & 0x0000ffff);
        ApplicationVolumeLevel = calculatedVolume / (ushort.MaxValue / 10);
    }

    private async Task Load()
    {
        await _libraryController.Initialize()
            .Do(library =>
            {
                Library = library;
                ElapsedRunningTime = 0;
                ElapsedRunningTimeDisplay = TimeSpan.ToString("mm':'ss");
            })
            .Do(_ => ProcessSourceFolders());
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

        _playerController.PlaySong(CurrentSong, AllSongsOrdered);
        var playingNow = AllSongs.SingleOrDefault(s => s.IsPlaying);
        if (playingNow is not null)
            playingNow.IsPlaying = false;

        CurrentSong.IsPlaying = true;

        NotifyOfPropertyChange(nameof(IsPlaying));
        NotifyOfPropertyChange(nameof(ElapsedRunningTimeDisplay));
        NotifyOfPropertyChange(nameof(ElapsedRunningTime));
        NotifyOfPropertyChange(nameof(CurrentSongPicture));
    }

    private void SearchSongs(string? artist, string? album, string? song)
    {
        var filteredSongs = AllSongs.AsEnumerable();

        if (artist != null)
            filteredSongs = AllSongs.Where(s => s.Artist.Contains(artist, StringComparison.OrdinalIgnoreCase));
        if (album != null)
            filteredSongs = AllSongs.Where(s => s.Album.Contains(album, StringComparison.OrdinalIgnoreCase));
        if (song != null)
            filteredSongs = AllSongs.Where(s =>
                s.Title != null && s.Title.Contains(song, StringComparison.OrdinalIgnoreCase));

        Songs = filteredSongs.ToObservableCollection();
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
            var folderPath = fbd.SelectedPath;

            IsLoadingSourceFolders = true;

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderPath))
                RunAsync(async () => await BuildSourceFolders(folderPath)
                    .Do(_ => ProcessSourceFolders())
                    .Bind(async _ => await Save(folderPath)));
            else
                IsLoadingSourceFolders = false;
        }
    }

    private async Task<NewResult<NewUnit>> Save(string folderPath)
    {
        return await _libraryController.SaveFolderPath(folderPath);
    }

    private async Task<NewResult<NewUnit>> BuildSourceFolders(string folderPath)
    {
        return await NewResultExtensions.Try(async () =>
        {
            await Task.Run(() =>
            {
                _sourceFolderController.BuildSourceFolders(folderPath, SourceFolders)
                    .Do(sourceFolders => _mediaController.LoadFromFolderPath(sourceFolders, Library!))
                    .Do(sourceFolders => SourceFolders = sourceFolders);
            }).ConfigureAwait(true);

            return NewUnit.Default;
        });
    }

    public void OpenBrowserToFolderPath()
    {
        if (_selectedTreeViewItem is null)
            return;

        Process.Start("explorer.exe", _selectedTreeViewItem.SourceFolder.FullPath);
    }

    public void SelectedTreeViewItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is SourceTreeViewItem treeViewItem)
            _selectedTreeViewItem = treeViewItem;
    }

    private void ProcessSourceFolders()
    {
        SourceTreeItems.Clear();

        var buildContextMenu = _contextMenuBuilder.BuildContextMenu(OpenBrowserToFolderPath,
            RemoveSourceFolder);
        foreach (var sourceFolder in SourceFolders)
            SourceTreeItems.Add(BuildTreeGridItem(sourceFolder, buildContextMenu));

        FilterSongs(SelectedArtist?.Name, SelectedAlbum?.Name);

        NotifyCollectionsChanged();

        IsLoadingSourceFolders = false;
    }

    private void RemoveSourceFolder()
    {
        if (_selectedTreeViewItem is null)
            return;

        var messageResult = MessageBox.Show(
            $"Are you sure you want to remove '{_selectedTreeViewItem.Header}'?",
            "Delete Source", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (messageResult == MessageBoxResult.Yes)
        {
            //TODO testing
            _sourceFolderController
                .Remove(Library!, _selectedTreeViewItem.SourceFolder)
                .Do(_ =>
                {
                    if (_selectedTreeViewItem.Parent is SourceTreeViewItem parent)
                        parent.Items.Remove(_selectedTreeViewItem);
                    else if (_selectedTreeViewItem.SourceFolder.IsRoot)
                        SourceTreeItems.Remove(_selectedTreeViewItem);
                    
                    NotifyCollectionsChanged();
                });
        }
    }

    private static SourceTreeViewItem BuildTreeGridItem(SourceFolder sourceFolder, ContextMenu contextMenu)
    {
        var treeItem = new SourceTreeViewItem
        {
            Header = sourceFolder.Header,
            ContextMenu = contextMenu,
            SourceFolder = sourceFolder
        };

        foreach (var sourceFolderItem in sourceFolder.Items)
            treeItem.Items.Add(BuildTreeGridItem(sourceFolderItem, contextMenu));

        return treeItem;
    }

    public void PreviousSong()
    {
        if (CurrentSong is null)
            return;

        _playerController.Previous(CurrentSong, AllSongsOrdered, ElapsedRunningTime);
    }

    public void NextSong()
    {
        if (CurrentSong is null)
            return;

        _playerController.Skip(CurrentSong, AllSongsOrdered);
    }
}