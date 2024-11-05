using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Enums;
using ShufflerPro.Client.Factories;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Result;
using ShufflerPro.Upgraded.Framework;
using ShufflerPro.Upgraded.Framework.WPF;
using MessageBox = System.Windows.MessageBox;
using Theme = ShufflerPro.Client.Entities.Theme;

namespace ShufflerPro.Upgraded.Screens.Shell;

public interface IShellViewModelFactory : IFactory
{
    ShellViewModel Create(Library library);
}

public class ShellViewModel : ViewModelBase
{
    private readonly AlbumArtLoader _albumArtLoader;
    private readonly BinaryHelper _binaryHelper;

    private readonly ContextMenuBuilder _contextMenuBuilder;
    private readonly Library _library;

    private readonly LibraryController _libraryController;
    private readonly MediaController _mediaController;
    private readonly PlayerController _playerController;
    private readonly RandomSongQueueFactory _randomSongQueueFactory;

    private readonly SongQueueFactory _songQueueFactory;

    private readonly SongStack _songStack;
    private readonly SourceFolderController _sourceFolderController;
    private double _applicationVolumeLevel;
    private Song? _currentSong;
    private double _elapsedRunningTime;
    private string _elapsedRunningTimeDisplay;
    private bool _isLoadingSourceFolders;
    private bool _isShuffleChecked;
    private LibrarySearchType _librarySearchType;
    private bool _playingPrevious;
    private string _searchText;
    private Album? _selectedAlbum;
    private Artist? _selectedArtist;
    private Song? _selectedSong;
    private Theme _selectedTheme;
    private SourceTreeViewItem? _selectedTreeViewItem;
    private ISongQueue? _songQueue;
    private ObservableCollection<Song> _songs;
    private ObservableCollection<SourceTreeViewItem> _sourceTreeItems;

    private CountDownTimer? _timer;

    public ShellViewModel(
        Library library,
        PlayerController playerController,
        SourceFolderController sourceFolderController,
        MediaController mediaController,
        BinaryHelper binaryHelper,
        LibraryController libraryController,
        ContextMenuBuilder contextMenuBuilder,
        SongQueueFactory songQueueFactory,
        AlbumArtLoader albumArtLoader, RandomSongQueueFactory randomSongQueueFactory, SongStack songStack)
    {
        _playerController = playerController;
        _sourceFolderController = sourceFolderController;
        _mediaController = mediaController;
        _binaryHelper = binaryHelper;
        _libraryController = libraryController;
        _contextMenuBuilder = contextMenuBuilder;
        _songQueueFactory = songQueueFactory;
        _albumArtLoader = albumArtLoader;
        _randomSongQueueFactory = randomSongQueueFactory;
        _songStack = songStack;
        _library = library;

        TimeSpan = new TimeSpan();

        _playerController.SongChanged += OnSongChanged;
        _playerController.PlayerDisposed += OnPlayerDisposed;
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
            NotifyOfPropertyChange(nameof(CurrentSongPicture));
            NotifyOfPropertyChange(nameof(HasAlbumArt));
        }
    }

    public ObservableCollection<Song> Songs
    {
        get => _songs;
        set
        {
            if (Equals(value, _songs)) return;
            _songs = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(SongSelectionSummary));
        }
    }

    public ObservableCollection<Album> Albums =>
        SelectedArtist?.Albums.OrderBy(a => a.Name).ToObservableCollection() ??
        AllAlbums.OrderBy(a => a.Name).ToObservableCollection();

    public IReadOnlyCollection<Artist> Artists => _library.Artists.OrderBy(a => a.Name).ToReadOnlyCollection();
    private IReadOnlyCollection<Song> AllSongs => _library.Songs;
    private IReadOnlyCollection<Album> AllAlbums => _library.Albums;

    public Artist? SelectedArtist
    {
        get => _selectedArtist;
        set
        {
            if (Equals(value, _selectedArtist)) return;
            _selectedArtist = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(Albums));

            HandleFilterSongs(value?.Name);
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

            HandleFilterSongs(SelectedArtist?.Name, value?.Name);
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

    public string SongSelectionSummary
    {
        get
        {
            var totalSongs = Songs.Count;
            var totalSpan = Songs
                .Aggregate(TimeSpan.Zero, (sumSoFar, nextMyObject) =>
                {
                    if (nextMyObject.Duration.HasValue)
                        return sumSoFar + nextMyObject.Duration.Value;
                    return sumSoFar;
                });
            return $"{totalSongs} songs, {totalSpan.Duration().StripMilliseconds()} total time";
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
        get => _library.SourceFolders;
        set
        {
            if (Equals(value, _library.SourceFolders)) return;
            _library.SourceFolders = value;
            NotifyOfPropertyChange();
        }
    }

    public bool IsPlaying => _playerController.Playing;

    public BitmapImage? CurrentSongPicture
    {
        get
        {
            var albumArt = _albumArtLoader.Load(CurrentSong?.Path);
            return _binaryHelper.ToImage(albumArt);
        }
    }

    public double ApplicationVolumeLevel
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
        }
    }

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

    public bool HasAlbumArt => CurrentSongPicture != null;

    public bool IsShuffleChecked
    {
        get => _isShuffleChecked;
        set
        {
            if (value == _isShuffleChecked) return;
            _isShuffleChecked = value;
            NotifyOfPropertyChange();
        }
    }

    public Theme SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (Equals(value, _selectedTheme)) return;
            _selectedTheme = value;
            NotifyOfPropertyChange();
            ThemeController.ChangeTheme(value);
        }
    }

    private void OnPlayerDisposed()
    {
        if (CurrentSong is not null)
            CurrentSong.IsPlaying = false;

        CurrentSong = null;

        NotifyOfPropertyChange(nameof(IsPlaying));
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

    private NewResult<NewUnit> WireTimer()
    {
        return NewResultExtensions.Try(() =>
        {
            _timer = new CountDownTimer();

            _timer.SetTime(CurrentSong!.Duration!.Value);
            _timer.TimeChanged += () =>
            {
                var timeSpan = CurrentSong!.Duration!.Value.Subtract(_timer.TimeLeft);

                ElapsedRunningTime = timeSpan.TotalSeconds;
                ElapsedRunningTimeDisplay = timeSpan.ToString("mm':'ss");
            };

            _timer.Start();

            return NewUnit.Default;
        });
    }

    private void NotifyCollectionsChanged()
    {
        NotifyOfPropertyChange(nameof(SelectedArtist));
        NotifyOfPropertyChange(nameof(Artists));
        NotifyOfPropertyChange(nameof(Albums));
        NotifyOfPropertyChange(nameof(Songs));
        NotifyOfPropertyChange(nameof(AllSongs));
        NotifyOfPropertyChange(nameof(AllAlbums));
        NotifyOfPropertyChange(nameof(SongSelectionSummary));
        NotifyOfPropertyChange(nameof(SourceFolders));
    }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        DisplayName = "Shuffler";
        SourceTreeItems = [];
        Songs = [];
        LibrarySearchType = LibrarySearchType.Artist;
        SelectedTheme = Themes.Default;
        InitializeApplicationVolume();
        StartLibrary();

        return base.OnInitializeAsync(cancellationToken);
    }

    private void StartLibrary()
    {
        ElapsedRunningTime.Reset();
        ElapsedRunningTimeDisplay.DefaultTimeSpan();
        ProcessSourceFolders();
    }

    private void InitializeApplicationVolume()
    {
        WinImport.waveOutGetVolume(IntPtr.Zero, out var currentVolume);

        var calculatedVolume = (ushort)(currentVolume & 0x0000ffff);
        ApplicationVolumeLevel = calculatedVolume / (ushort.MaxValue / 10d);
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
        if (_songQueue?.CurrentSong is null)
        {
            PlaySong();
            return;
        }

        if (_playerController.Playing)
        {
            _timer?.Pause();
            _playerController.Pause();
        }
        else
        {
            _timer?.Start();
            _playerController.Resume();
        }

        NotifyOfPropertyChange(nameof(IsPlaying));
    }

    public void AdjustApplicationVolume()
    {
        var newVolume = ushort.MaxValue / 10d * ApplicationVolumeLevel;
        var newVolumeAllChannels = ((uint)newVolume & 0x0000ffff) | ((uint)newVolume << 16);

        WinImport.waveOutSetVolume(IntPtr.Zero, newVolumeAllChannels);
    }

    public NewResult<NewUnit> InitializePlaySong()
    {
        return NewResultExtensions.Try(() =>
            {
                if (_playerController.Playing || _playerController.IsPaused)
                    _playerController.Cancel();

                ElapsedRunningTime.Reset();

                return SelectedSong;
            })
            .Bind(currentSong => BuildSongQueue(currentSong!)
                .Do(songQueue =>
                {
                    _songQueue = songQueue;
                    CurrentSong = _songQueue.CurrentSong;
                })
                .Bind(_ => WireTimer()));
    }

    private NewResult<ISongQueue> BuildSongQueue(Song currentSong)
    {
        return NewResultExtensions.Try(() =>
        {
            if (IsShuffleChecked)
            {
                var songQueue = _randomSongQueueFactory
                    .Create(new RandomSongQueueState(currentSong, Songs,
                        _songStack, _playingPrevious));
                _playingPrevious = false;
                return songQueue;
            }

            return _songQueueFactory.Create(currentSong, Songs);
        });
    }

    public void PlaySong()
    {
        HandleSelectedSong()
            .IfFail(exception => MessageBox.Show(exception.Message))
            .IfSuccess(_ => InitializePlaySong()
                .IfSuccess(_ =>
                {
                    _playerController.PlaySong(_songQueue!);

                    var playingNow = AllSongs.SingleOrDefault(s => s.IsPlaying);
                    if (playingNow is not null)
                        playingNow.IsPlaying = false;

                    CurrentSong!.IsPlaying = true;

                    NotifyOfPropertyChange(nameof(IsPlaying));
                    NotifyOfPropertyChange(nameof(ElapsedRunningTimeDisplay));
                    NotifyOfPropertyChange(nameof(ElapsedRunningTime));
                    NotifyOfPropertyChange(nameof(CurrentSongPicture));
                    NotifyOfPropertyChange(nameof(HasAlbumArt));
                }));
    }

    private NewResult<NewResult<NewUnit>> HandleSelectedSong()
    {
        return NewResultExtensions.Try(() =>
        {
            if (SelectedSong is null)
            {
                var firstSong = Songs.FirstOrDefault();
                if (firstSong is null)
                    return NewResultExtensions.CreateFail<NewUnit>();

                SelectedSong = firstSong;
            }

            return NewUnit.Default;
        });
    }

    private IEnumerable<Song> FilterSongs(string? artist, string? album)
    {
        var filteredSongs = AllSongs.AsEnumerable();

        if (artist != null && album == null)
            filteredSongs = AllSongs.Where(s => s.Artist == artist);
        if (artist == null && album != null)
            filteredSongs = AllSongs.Where(s => s.Album == album);
        if (artist != null && album != null)
            filteredSongs = AllSongs.Where(s => s.Artist == artist && s.Album == album);
        return filteredSongs;
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

    private void HandleFilterSongs(string? artist = null, string? album = null)
    {
        var filteredSongs = FilterSongs(artist, album);
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
                    .Bind(async state => await InsertSource(state)));
            else
                IsLoadingSourceFolders = false;
        }
    }

    private async Task<NewResult<NewUnit>> InsertSource(SourceFolderState state)
    {
        return await _libraryController.InsertSource(state);
    }

    private async Task<NewResult<SourceFolderState>> BuildSourceFolders(string folderPath)
    {
        return await NewResultExtensions.Try(async () =>
        {
            var state = new SourceFolderState(SourceFolders);
            await Task.Run(() =>
            {
                _sourceFolderController.BuildFromPath(folderPath, state)
                    .Do(_ => _mediaController.LoadFromFolderPath(state.SourceFolders, _library))
                    .Do(_ => SourceFolders = state.SourceFolders);
            }).ConfigureAwait(true);
            return state;
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

        var buildContextMenu = _contextMenuBuilder.BuildContextMenu(OpenBrowserToFolderPath, RemoveSourceFolder);
        foreach (var sourceFolder in SourceFolders)
            SourceTreeItems.Add(BuildTreeGridItem(sourceFolder, buildContextMenu));

        HandleFilterSongs(SelectedArtist?.Name, SelectedAlbum?.Name);
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
            RunAsync(async () => await _sourceFolderController.Remove(_library, _selectedTreeViewItem.SourceFolder)
                .IfFail(exception => MessageBox.Show(exception.Message))
                .IfSuccess(_ =>
                {
                    if (_selectedTreeViewItem.Parent is SourceTreeViewItem parent)
                    {
                        parent.Items.Remove(_selectedTreeViewItem);

                        if (parent.Items.Count == 0)
                            SourceTreeItems.Remove(parent);
                    }
                    else if (_selectedTreeViewItem.SourceFolder.IsRoot)
                    {
                        SourceTreeItems.Remove(_selectedTreeViewItem);
                    }

                    HandleFilterSongs(SelectedArtist?.Name, SelectedAlbum?.Name);
                    NotifyCollectionsChanged();
                }));
    }

    private static SourceTreeViewItem BuildTreeGridItem(SourceFolder sourceFolder, ContextMenu contextMenu)
    {
        var treeItem = new SourceTreeViewItem
        {
            Header = sourceFolder.Header,
            ContextMenu = contextMenu,
            SourceFolder = sourceFolder
        };

        if (!sourceFolder.IsValid)
        {
            treeItem.Foreground = Brushes.Red;
            treeItem.ToolTip = $"Failed to connect to path '{sourceFolder.FullPath}'";
        }

        foreach (var sourceFolderItem in sourceFolder.Items)
            treeItem.Items.Add(BuildTreeGridItem(sourceFolderItem, contextMenu));

        return treeItem;
    }

    public void PreviousSong()
    {
        if (CurrentSong is null || _songQueue is null)
            return;
        
        if(ElapsedRunningTime >= 5)
            OnSongChanged(CurrentSong);

        _playingPrevious = true;
        _playerController.Previous(_songQueue);
    }

    public void NextSong()
    {
        if (CurrentSong is null || _songQueue is null)
            return;

        _playerController.Skip(_songQueue);
    }
}