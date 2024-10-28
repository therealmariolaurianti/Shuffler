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
    private ObservableCollection<Album>? _albums;
    private Album? _selectedAlbum;
    private Artist? _selectedArtist;
    private Song? _selectedSong;
    private ObservableCollection<Song>? _songs;

    public ShellViewModel(PlayerController playerController, MediaController mediaController)
    {
        _playerController = playerController;
        _mediaController = mediaController;
    }

    public Song? CurrentSong { get; set; }

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
            CurrentSong = value;
        }
    }

    // protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
    // {
    
    //
    //     await base.OnInitializeAsync(cancellationToken);
    // }

    protected override Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        DisplayName = "Shuffler Pro";
        Load();
        
        return base.OnInitializeAsync(cancellationToken);
    }

    private void Load()
    {
        Artists = _mediaController.LoadArtists();

        Songs = AllSongs?.ToObservableCollection();
        Albums = AllAlbums?.ToObservableCollection();
    }

    public void PlaySong()
    {
        if (CurrentSong is null)
            return;

        if (_playerController.Playing)
            _playerController.Cancel();

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