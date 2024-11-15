using System.Collections;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using JetBrains.Annotations;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Factories;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Framework;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;

namespace ShufflerPro.Screens.ExcludedSongs;

public interface IExcludedSongsViewModelFactory : IFactory
{
    ExcludedSongsViewModel Create(Library library);
}

public class ExcludedSongsViewModel : ViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    private readonly Library _library;
    private readonly SongController _songController;
    private readonly ShufflerWindowManager _windowManager;
    private ObservableCollection<Song> _excludedSongs;
    private IList? _selectedSongs;

    public ExcludedSongsViewModel(
        Library library,
        ShufflerWindowManager windowManager,
        SongController songController, IEventAggregator eventAggregator)
    {
        _library = library;
        _windowManager = windowManager;
        _songController = songController;
        _eventAggregator = eventAggregator;
        ExcludedSongs = [];

        Start();
    }

    public ObservableCollection<Song> ExcludedSongs
    {
        get => _excludedSongs;
        set
        {
            if (Equals(value, _excludedSongs)) return;
            _excludedSongs = value;
            NotifyOfPropertyChange();
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
            NotifyOfPropertyChange(nameof(CanAddToLibrary));
        }
    }

    public bool CanAddToLibrary => _songs.Count > 0;

    private List<Song> _songs => SelectedSongs?.Cast<Song>().ToList() ?? [];

    private void Start()
    {
        var songs = _library.ExcludedSongs
            .Select(excludedSong => SongFactory.Create(excludedSong.SongPath));

        foreach (var song in songs)
            ExcludedSongs.Add(song);
    }

    [UsedImplicitly]
    public void AddToLibrary()
    {
        RunAsync(async () =>
        {
            await _songController.RemoveExcludedSongs(_songs, _library)
                .IfFail(exception => _windowManager.ShowMessageBox(exception))
                .IfSuccessAsync(async _ =>
                {
                    foreach (var selectedSong in _songs)
                        ExcludedSongs.Remove(selectedSong);

                    NotifyOfPropertyChange(nameof(ExcludedSongs));

                    await _eventAggregator.PublishOnBackgroundThreadAsync(new SongAction());
                });
        });
    }
}