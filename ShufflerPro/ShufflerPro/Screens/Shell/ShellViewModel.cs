using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using Microsoft.Xaml.Behaviors.Core;
using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Enums;
using ShufflerPro.Client.Extensions;
using ShufflerPro.Client.Factories;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Client.States;
using ShufflerPro.Framework;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;
using DragDropEffects = System.Windows.DragDropEffects;
using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;
using MessageBox = System.Windows.MessageBox;

namespace ShufflerPro.Screens.Shell;

public class ShellViewModel : ViewModelBase, IHandle<SongAction>, IDisposable, IDropTarget
{
    private readonly AlbumArtLoader _albumArtLoader;
    private readonly ContextMenuBuilder _contextMenuBuilder;
    private readonly HotKeyListener _hotKeyListener;
    private readonly Library _library;
    private readonly LibraryController _libraryController;
    private readonly MediaController _mediaController;
    private readonly PlayerController _playerController;
    private readonly PlaylistController _playlistController;
    private readonly RandomSongQueueFactory _randomSongQueueFactory;
    private readonly SongController _songController;
    private readonly SongFilterController _songFilterController;
    private readonly SongQueueFactory _songQueueFactory;
    private readonly SongStack _songStack;
    private readonly SourceFolderController _sourceFolderController;
    private readonly ShufflerWindowManager _windowManager;
    private double _applicationVolumeLevel;
    private Song? _currentSong;
    private TimeSpan _currentSongTime;
    private double _elapsedRunningTime;
    private string? _elapsedRunningTimeDisplay;
    private bool _isLoadingSourceFolders;


    private bool _isMuted;
    private bool _isRepeatChecked;
    private bool _isShuffleChecked;
    private bool _isSliderBeingDragged;
    private LibrarySearchType _librarySearchType;
    private bool _playingPrevious;
    private PlaylistState? _playlistState;
    private RepeatType _repeatType;
    private string? _searchText;
    private Album? _selectedAlbum;
    private Artist? _selectedArtist;
    private PlaylistGridItem? _selectedPlaylist;
    private Song? _selectedSong;
    private IList? _selectedSongs;
    private double _selectedSongTime;
    private SourceTreeViewItem? _selectedTreeViewItem;
    private ISongQueue? _songQueue;
    private ObservableCollection<Song>? _songs;
    private ObservableCollection<SourceTreeViewItem>? _sourceTreeItems;
    private double _startingSongTime;
    private CountDownTimer? _timer;
    private double _volumeLevelBeforeMute;

    public ShellViewModel(
        Library library,
        PlayerController playerController,
        SourceFolderController sourceFolderController,
        MediaController mediaController,
        LibraryController libraryController,
        ContextMenuBuilder contextMenuBuilder,
        SongQueueFactory songQueueFactory,
        AlbumArtLoader albumArtLoader,
        RandomSongQueueFactory randomSongQueueFactory,
        SongStack songStack,
        PlaylistController playlistController,
        SongFilterController songFilterController,
        ShufflerWindowManager windowManager,
        SongController songController,
        IEventAggregator eventAggregator, HotKeyListener hotKeyListener)
    {
        _playerController = playerController;
        _sourceFolderController = sourceFolderController;
        _mediaController = mediaController;
        _libraryController = libraryController;
        _contextMenuBuilder = contextMenuBuilder;
        _songQueueFactory = songQueueFactory;
        _albumArtLoader = albumArtLoader;
        _randomSongQueueFactory = randomSongQueueFactory;
        _songStack = songStack;
        _playlistController = playlistController;
        _songFilterController = songFilterController;
        _windowManager = windowManager;
        _songController = songController;
        _hotKeyListener = hotKeyListener;
        _library = library;

        TimeSpan = new TimeSpan();

        _playerController.SongChanged += OnSongChanged;
        _playerController.PlayerDisposed += OnPlayerDisposed;

        WireHotKeys();

        EditPlaylistItemCommand = new ActionCommand(RenamePlaylist);
        EditLostFocusCommand = new ActionCommand(EditingItemLostFocus);

        eventAggregator.SubscribeOnBackgroundThread(this);
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
            NotifyOfPropertyChange(nameof(AlbumArt));
            NotifyOfPropertyChange(nameof(HasAlbumArt));
            NotifyOfPropertyChange(nameof(CanEditSong));
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
            NotifyCollectionsChanged();
        }
    }

    public ObservableCollection<Album> Albums
    {
        get
        {
            if (_playlistState is not null)
                return _playlistState.FilterAlbums(SelectedArtist);

            return SelectedArtist?.Albums.Distinct().OrderBy(a => a.Name).ToObservableCollection() ??
                   AllAlbums.Distinct().OrderBy(a => a.Name).ToObservableCollection();
        }
    }

    public IReadOnlyCollection<Artist> Artists => _playlistState?.Artists ?? _library
        .Artists
        .OrderBy(a => a.Name).ToReadOnlyCollection();

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
            NotifyOfPropertyChange(nameof(IsRemoveSongFromPlaylistVisible));
            NotifyOfPropertyChange(nameof(CanEditSong));
        }
    }

    public string SongSelectionSummary
    {
        get
        {
            var totalSongs = Songs?.Count;
            var totalSpan = Songs?
                .Aggregate(TimeSpan.Zero, (sumSoFar, nextMyObject) =>
                {
                    if (nextMyObject.Duration.HasValue)
                        return sumSoFar + nextMyObject.Duration.Value;
                    return sumSoFar;
                });
            if (totalSpan is null)
                return "Library is Empty";

            return $"{totalSongs} songs, {totalSpan.Value.Duration().StripMilliseconds()} total time";
        }
    }

    public double MaxRunTime => CurrentSong?.Duration?.TotalSeconds ?? 1;

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

    public string? ElapsedRunningTimeDisplay
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

    public ObservableCollection<SourceTreeViewItem>? SourceTreeItems
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

    public BitmapImage? AlbumArt => _albumArtLoader.Load(CurrentSong?.Path);

    public double ApplicationVolumeLevel
    {
        get => _applicationVolumeLevel;
        set
        {
            if (value.Equals(_applicationVolumeLevel)) return;
            _applicationVolumeLevel = value;
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

    public string? SearchText
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

    public bool HasAlbumArt => AlbumArt != null;

    public bool IsShuffleChecked
    {
        get => _isShuffleChecked;
        set
        {
            if (value == _isShuffleChecked) return;
            _isShuffleChecked = value;
            NotifyOfPropertyChange();
            if (IsRepeatChecked && IsShuffleChecked)
                IsRepeatChecked = false;
        }
    }


    public bool IsRepeatChecked
    {
        get => _isRepeatChecked;
        set
        {
            if (value == _isRepeatChecked) return;
            _isRepeatChecked = value;
            NotifyOfPropertyChange();
            if (IsShuffleChecked && IsRepeatChecked)
                IsShuffleChecked = false;
        }
    }

    public ObservableCollection<PlaylistGridItem> Playlists => _library
        .Playlists
        .Select(p => new PlaylistGridItem(p))
        .ToObservableCollection();

    public PlaylistGridItem? SelectedPlaylist
    {
        get => _selectedPlaylist;
        set
        {
            if (Equals(value, _selectedPlaylist)) return;
            _selectedPlaylist = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(CanRenamePlaylist));
            NotifyOfPropertyChange(nameof(CanRemovePlaylist));
            NotifyOfPropertyChange(nameof(IsRemoveSongFromPlaylistVisible));

            SelectedPlaylistChanged();
        }
    }

    public IList? SelectedSongs
    {
        get => _selectedSongs;
        set
        {
            if (Equals(value, _selectedSongs)) return;
            _selectedSongs = value;
            NotifyOfPropertyChange();
        }
    }

    public double StartingSongTime
    {
        get => _startingSongTime;
        set
        {
            if (value.Equals(_startingSongTime)) return;
            _startingSongTime = value;
            NotifyOfPropertyChange();
        }
    }

    public double SelectedSongTime
    {
        get => _selectedSongTime;
        set
        {
            if (value.Equals(_selectedSongTime)) return;
            _selectedSongTime = value;
            NotifyOfPropertyChange();

            if (!IsSliderBeingDragged)
                HandleSelectedTime();
            else
                ElapsedRunningTimeDisplay = TimeSpan.FromSeconds(value).ToString("mm':'ss");
        }
    }

    public ICommand EditPlaylistItemCommand { get; }
    public ICommand EditLostFocusCommand { get; }

    public bool CanRenamePlaylist => SelectedPlaylist != null;
    public bool CanRemovePlaylist => SelectedPlaylist != null;
    public bool IsRemoveSongFromPlaylistVisible => SelectedPlaylist != null && SelectedSong != null;

    public bool IsSliderBeingDragged
    {
        get => _isSliderBeingDragged;
        set
        {
            if (value == _isSliderBeingDragged) return;
            _isSliderBeingDragged = value;
            NotifyOfPropertyChange();
        }
    }

    public RepeatType RepeatType
    {
        get => _repeatType;
        set
        {
            if (value == _repeatType) return;
            _repeatType = value;
            NotifyOfPropertyChange();
        }
    }

    public bool CanEditSong => SelectedSong?.Id != CurrentSong?.Id;

    public void Dispose()
    {
        _playerController.Dispose();
        _hotKeyListener.Dispose();
        _timer?.Dispose();
        _hotKeyListener.Dispose();
    }


    void IDropTarget.DragOver(IDropInfo dropInfo)
    {
        var sourceItem = dropInfo.Data;
        if (sourceItem is null)
            return;

        dropInfo.Effects = DragDropEffects.Copy;

        switch (dropInfo.TargetItem)
        {
            case PlaylistGridItem:
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                break;
            case Song:
            {
                if (SelectedPlaylist is null)
                    return;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
                break;
        }
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
    {
        var songs = new List<Song>();

        if (dropInfo.Data.GetType() == typeof(List<object>))
        {
            var dropInfoData = dropInfo.Data as List<object>;
            songs = dropInfoData?.Cast<Song>().ToList();
        }
        else if (dropInfo.Data is Song song)
        {
            songs.Add(song);
        }

        if (songs!.Count == 0 || dropInfo.Data is null)
            return;

        switch (dropInfo.TargetItem)
        {
            case PlaylistGridItem playlistGridItem:
                HandleDropSongOnPlaylist(playlistGridItem, songs);
                break;
            case Song song:
                HandleMoveSongInPlaylist(song, (dropInfo.Data as Song)!);
                break;
        }
    }

    async Task IHandle<SongAction>
        .HandleAsync(SongAction message, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            HandleFilterSongs(SelectedArtist?.Name, SelectedAlbum?.Name);
            NotifyCollectionsChanged();
        }, cancellationToken);
    }

    private void HandleMoveSongInPlaylist(Song target, Song source)
    {
        if (Songs == null || SelectedPlaylist is null || _playlistState is null)
            return;

        var index = Songs.IndexOf(target);

        RunAsync(async () => await _playlistController
            .MoveSong(_playlistState, source, index)
            .IfFailAsync(async exception => await _windowManager.ShowException(exception)));
    }

    private void HandleDropSongOnPlaylist(PlaylistGridItem playlistGridItem, List<Song> songs)
    {
        RunAsync(async () => await _playlistController
            .AddSongs(playlistGridItem.Item, songs));
    }

    private void WireHotKeys()
    {
        _hotKeyListener.OnNextTrack += NextSong;
        _hotKeyListener.OnPreviousTrack += PreviousSong;
        _hotKeyListener.OnPlayPause += PlayPause;
        _hotKeyListener.OnMute += Mute;
    }

    [UsedImplicitly]
    private void Mute()
    {
        if (!_isMuted)
        {
            _volumeLevelBeforeMute = ApplicationVolumeLevel;

            ApplicationVolumeLevel = 0;
            AdjustApplicationVolume();

            _isMuted = true;
        }
        else
        {
            _isMuted = false;
            ApplicationVolumeLevel = _volumeLevelBeforeMute;

            AdjustApplicationVolume();
        }
    }

    private void HandleSelectedTime()
    {
        if (_timer is not { IsRunning: true })
            return;

        var timeSpan = TimeSpan.FromSeconds(SelectedSongTime);

        _playerController.SetCurrentTime(timeSpan);

        _timer.Pause();
        {
            SetElapsedRunTimeDisplay(timeSpan);
            _currentSongTime = timeSpan;
        }
        _timer.Start();
    }

    private void SelectedPlaylistChanged()
    {
        SelectedSong = null;
        SelectedArtist = null;
        SelectedAlbum = null;

        if (SelectedPlaylist is null)
        {
            _playlistState = null;
            HandleFilterSongs();
        }
        else
        {
            _songFilterController
                .FilterSongs(AllSongs, SelectedPlaylist.Item)
                .Do(playlistState =>
                {
                    _playlistState = playlistState;
                    Songs = playlistState.Songs;
                });
        }

        NotifyOfPropertyChange(nameof(Artists));
        NotifyOfPropertyChange(nameof(Albums));
        NotifyOfPropertyChange(nameof(Songs));
        NotifyOfPropertyChange(nameof(SongSelectionSummary));
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
        Task.Run(() =>
        {
            Songs = LibrarySearchType switch
            {
                LibrarySearchType.Artist => _songFilterController.SearchSongs(AllSongs, SearchText, null, null),
                LibrarySearchType.Song => _songFilterController.SearchSongs(AllSongs, null, null, SearchText),
                LibrarySearchType.Album => _songFilterController.SearchSongs(AllSongs, null, SearchText, null),
                _ => throw new ArgumentOutOfRangeException()
            };
        });
    }

    private NewResult<NewUnit> WireTimer()
    {
        return NewResultExtensions.Try(() =>
        {
            if (_timer?.IsRunning ?? false)
                _timer.Stop();

            _timer = new CountDownTimer();

            _timer.SetTime(CurrentSong!.Duration!.Value);
            _timer.TimeChanged += OnTimeChanged;

            _timer.Start();

            return NewUnit.Default;
        });
    }

    private void OnTimeChanged()
    {
        if (CurrentSong is null || _timer is null)
            return;

        _currentSongTime = _currentSongTime.Tick();
        SetElapsedRunTimeDisplay(_currentSongTime);
    }

    private void SetElapsedRunTimeDisplay(TimeSpan timeSpan)
    {
        if (!IsSliderBeingDragged)
        {
            ElapsedRunningTime = timeSpan.TotalSeconds;
            ElapsedRunningTimeDisplay = timeSpan.ToString("mm':'ss");
        }
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
        SourceTreeItems = [];
        Songs = [];
        LibrarySearchType = LibrarySearchType.Artist;

        InitializeApplicationVolume();
        StartLibrary();

        NotifyCollectionsChanged();

        return base.OnInitializeAsync(cancellationToken);
    }

    private void StartLibrary()
    {
        ResetCurrentElapsed();
        ProcessSourceFolders();
    }

    private void ResetCurrentElapsed()
    {
        Task.Run(() =>
        {
            ElapsedRunningTime = 0;
            ElapsedRunningTimeDisplay = TimeSpan.Zero.ToString("mm':'ss");
        });
    }

    private void InitializeApplicationVolume()
    {
        WinImport.waveOutGetVolume(IntPtr.Zero, out var currentVolume);

        var calculatedVolume = (ushort)(currentVolume & 0x0000ffff);
        ApplicationVolumeLevel = calculatedVolume / (ushort.MaxValue / 10d);
    }

    [UsedImplicitly]
    public void PlayArtist()
    {
        var firstSong = SelectedArtist?.Songs.OrderBy(s => s.Artist)
            .ThenBy(s => s.Album)
            .ThenBy(s => s.Track)
            .FirstOrDefault();

        SelectedSong = firstSong;
        PlaySong();
    }

    [UsedImplicitly]
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

    [UsedImplicitly]
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

    [UsedImplicitly]
    public void AdjustApplicationVolume()
    {
        if (_isMuted)
            _isMuted = false;

        var newVolumeAllChannels = CalculateVolumeAllChannels(ApplicationVolumeLevel);
        _ = WinImport.waveOutSetVolume(IntPtr.Zero, newVolumeAllChannels);
    }

    private static uint CalculateVolumeAllChannels(double applicationVolumeLevel)
    {
        var newVolume = ushort.MaxValue / 10d * applicationVolumeLevel;
        var lowOrderBits = (uint)newVolume & 0x0000ffff;
        var highOrderBits = (uint)newVolume << 16;
        return lowOrderBits | highOrderBits;
    }

    public NewResult<NewUnit> InitializePlaySong(bool isSourceGrid)
    {
        return NewResultExtensions.Try(() =>
            {
                if (_playerController.Playing || _playerController.IsPaused)
                    _playerController.Cancel();

                ElapsedRunningTime = 0;

                return SelectedSong;
            })
            .Bind(currentSong => BuildSongQueue(currentSong!, isSourceGrid)
                .Do(songQueue =>
                {
                    _songQueue = songQueue;
                    CurrentSong = _songQueue.CurrentSong;
                })
                .Bind(_ => WireTimer()));
    }

    private NewResult<ISongQueue> BuildSongQueue(Song currentSong, bool isSourceGrid)
    {
        return NewResultExtensions.Try(() =>
        {
            if (IsShuffleChecked)
                return ShuffleSongs(currentSong, isSourceGrid);

            var observableCollection = _playlistState is null
                ? _songFilterController.FilterSongs(AllSongs, null, null)
                : _playlistState.Songs;
            
            return _songQueueFactory.Create(currentSong, observableCollection,
                new RepeatState(IsRepeatChecked, RepeatType));
        });
    }

    private ISongQueue ShuffleSongs(Song currentSong, bool isSourceGrid)
    {
        var songQueue = _randomSongQueueFactory
            .Create(new RandomSongQueueState(currentSong, Songs,
                _songStack, _playingPrevious, isSourceGrid));
        _playingPrevious = false;
        return songQueue;
    }

    [UsedImplicitly]
    public void GridDoubleClicked(object sender, MouseButtonEventArgs e)
    {
        PlaySong(true);
    }

    public void PlaySong(bool isSourceGrid = false)
    {
        HandleSelectedSong()
            .IfSuccess(_ => InitializePlaySong(isSourceGrid)
                .IfSuccess(_ =>
                {
                    _playerController.PlaySong(_songQueue!);

                    var playingNow = AllSongs.SingleOrDefault(s => s.IsPlaying);
                    if (playingNow is not null)
                        playingNow.IsPlaying = false;

                    CurrentSong!.IsPlaying = true;

                    NotifyInterfaceChanged();
                }));
    }

    private void NotifyInterfaceChanged()
    {
        NotifyOfPropertyChange(nameof(IsPlaying));
        NotifyOfPropertyChange(nameof(ElapsedRunningTimeDisplay));
        NotifyOfPropertyChange(nameof(ElapsedRunningTime));
        NotifyOfPropertyChange(nameof(AlbumArt));
        NotifyOfPropertyChange(nameof(HasAlbumArt));
    }

    private NewResult<NewUnit> HandleSelectedSong()
    {
        if (SelectedSong is null)
        {
            var firstSong = Songs?.FirstOrDefault();
            if (firstSong is null)
                return NewResultExtensions.CreateFail<NewUnit>(new Exception("No song to plays"));

            SelectedSong = firstSong;
        }

        ResetCurrentElapsed();
        _currentSongTime = TimeSpan.Zero;

        return NewUnit.Default;
    }

    private void HandleFilterSongs(string? artist = null, string? album = null)
    {
        Songs = SelectedPlaylist is not null && _playlistState is not null
            ? _songFilterController.FilterPlaylist(_playlistState, artist, album)
            : _songFilterController.FilterSongs(AllSongs, artist, album);
    }

    [UsedImplicitly]
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
                    .Do(_ => _mediaController.LoadFromFolderPath(state.SourceFolders, _library, _library.ExcludedSongs))
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

    [UsedImplicitly]
    public void SelectedTreeViewItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is SourceTreeViewItem treeViewItem)
            _selectedTreeViewItem = treeViewItem;
    }

    private void ProcessSourceFolders()
    {
        SourceTreeItems?.Clear();

        var buildContextMenu = _contextMenuBuilder.BuildContextMenu(OpenBrowserToFolderPath, RemoveSourceFolder);
        foreach (var sourceFolder in SourceFolders)
            SourceTreeItems?.Add(BuildTreeGridItem(sourceFolder, buildContextMenu));

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
                .IfFailAsync(async exception => await _windowManager.ShowException(exception))
                .IfSuccess(_ =>
                {
                    if (_selectedTreeViewItem.Parent is SourceTreeViewItem parent)
                    {
                        parent.Items.Remove(_selectedTreeViewItem);
                        ClearEmptyParents(parent);
                    }
                    else if (_selectedTreeViewItem.SourceFolder.IsRoot)
                    {
                        SourceTreeItems?.Remove(_selectedTreeViewItem);
                    }

                    HandleFilterSongs(SelectedArtist?.Name, SelectedAlbum?.Name);
                    NotifyCollectionsChanged();
                }));
    }

    private void ClearEmptyParents(SourceTreeViewItem parent)
    {
        while (true)
        {
            if (parent.Items.Count == 0)
                switch (parent.Parent)
                {
                    case SourceTreeViewItem parentItem:
                        parentItem.Items.Remove(parent);
                        parent = parentItem;
                        continue;
                    case null:
                        SourceTreeItems?.Remove(parent);
                        break;
                }

            break;
        }
    }

    private static SourceTreeViewItem BuildTreeGridItem(SourceFolder sourceFolder, ContextMenu contextMenu)
    {
        var treeItem = CreateSourceTreeViewItem(sourceFolder, contextMenu);

        foreach (var sourceFolderItem in sourceFolder.Items)
            treeItem.Items.Add(BuildTreeGridItem(sourceFolderItem, contextMenu));

        return treeItem;
    }

    private static SourceTreeViewItem CreateSourceTreeViewItem(SourceFolder sourceFolder, ContextMenu contextMenu)
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

        return treeItem;
    }

    [UsedImplicitly]
    public void PreviousSong()
    {
        if (CurrentSong is null || _songQueue is null)
            return;

        if (ElapsedRunningTime >= 5)
            OnSongChanged(CurrentSong);

        _playingPrevious = true;
        _playerController.Previous(_songQueue);
    }

    [UsedImplicitly]
    public void NextSong()
    {
        if (CurrentSong is null || _songQueue is null)
            return;

        ResetCurrentElapsed();

        _playerController
            .Skip(_songQueue)
            .IfFail(_ => EndPlaySong());
    }

    private void EndPlaySong()
    {
        _timer?.Stop();
        ResetCurrentElapsed();
        NotifyInterfaceChanged();
    }

    [UsedImplicitly]
    public void LaunchGitHubSite()
    {
        WebsiteLauncher.OpenWebsite("https://github.com/therealmariolaurianti/Shuffler");
    }


    [UsedImplicitly]
    public void LaunchSettings()
    {
        RunAsync(async () => await _windowManager.LaunchSettings(_library));
    }

    [UsedImplicitly]
    public void AddPlaylist()
    {
        RunAsync(async () => await _playlistController
            .AddPlaylist(_library, Playlist.Default)
            .Do(_ => NotifyOfPropertyChange(nameof(Playlists))));
    }

    public void RenamePlaylist()
    {
        if (SelectedPlaylist is null)
            return;
        SelectedPlaylist.IsInEditMode = true;
    }

    public void EditingItemLostFocus()
    {
        RunAsync(async () => await _playlistController.Update(SelectedPlaylist!.Item)
            .Do(_ =>
            {
                SelectedPlaylist!.IsInEditMode = false;
                NotifyOfPropertyChange(nameof(Playlists));
            }));
    }

    [UsedImplicitly]
    public void RemovePlaylist()
    {
        RunAsync(async () => await _playlistController.Delete(_library, SelectedPlaylist!.Item)
            .Do(_ => NotifyOfPropertyChange(nameof(Playlists))));
    }

    [UsedImplicitly]
    public void RemoveSongFromPlaylist()
    {
        RunAsync(async () =>
        {
            await _playlistController
                .RemoveSong(SelectedPlaylist!.Item, _playlistState!, SelectedSong!)
                .Do(_ =>
                {
                    NotifyOfPropertyChange(nameof(Songs));
                    NotifyOfPropertyChange(nameof(Artists));
                    NotifyOfPropertyChange(nameof(Albums));
                });
        });
    }

    [UsedImplicitly]
    public void EditSong()
    {
        if (SelectedSong is null || SelectedSongs?.Count == 0)
            return;

        RunAsync(async () =>
        {
            if (SelectedSongs!.Count == 1)
                await ShowEditSong();
            else
                await ShowEditSongs();
        });
    }

    private async Task ShowEditSongs()
    {
        var songs = SelectedSongs!.Cast<Song>().ToList();
        await _windowManager
            .ShowEditSongs(songs, _library)
            .IfSuccess(_ =>
            {
                HandleFilterSongs(SelectedArtist?.Name, SelectedAlbum?.Name);
                NotifyCollectionsChanged();
            });
    }

    private async Task ShowEditSong()
    {
        await _windowManager
            .ShowEditSong(SelectedSong!, _albumArtLoader.Load(SelectedSong!.Path), _library)
            .IfSuccess(_ =>
            {
                HandleFilterSongs(SelectedArtist?.Name, SelectedAlbum?.Name);
                NotifyCollectionsChanged();
            });
    }

    [UsedImplicitly]
    public void RemoveSongFromLibrary()
    {
        RunAsync(async () =>
        {
            var songs = SelectedSongs!.Cast<Song>().ToList();
            await _songController
                .Remove(songs, _library, _playlistState)
                .IfFailAsync(async exception => await _windowManager.ShowException(exception))
                .IfSuccess(_ =>
                {
                    HandleFilterSongs(SelectedArtist?.Name, SelectedAlbum?.Name);
                    NotifyCollectionsChanged();
                });
        });
    }
}