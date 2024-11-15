using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ShufflerPro.Client.Entities;
using ShufflerPro.Result;
using ShufflerPro.Screens.EditSong.Multiple;
using ShufflerPro.Screens.EditSong.Single;
using ShufflerPro.Screens.Exceptions;
using ShufflerPro.Screens.ExcludedSongs;
using ShufflerPro.Screens.Setting;

namespace ShufflerPro.Framework;

public class ShufflerWindowManager : WindowManager
{
    private readonly IEditSongsViewModelFactory _editSongsViewModelFactory;
    private readonly IEditSongViewModelFactory _editSongViewModelFactory;
    private readonly IExceptionViewModelFactory _exceptionViewModelFactory;
    private readonly IExcludedSongsViewModelFactory _excludedSongsViewModelFactory;
    private readonly ISettingsViewModelFactory _settingsViewModelFactory;

    public ShufflerWindowManager(
        IEditSongViewModelFactory editSongViewModelFactory,
        ISettingsViewModelFactory settingsViewModelFactory,
        IEditSongsViewModelFactory editSongsViewModelFactory,
        IExcludedSongsViewModelFactory excludedSongsViewModelFactory,
        IExceptionViewModelFactory exceptionViewModelFactory)
    {
        _editSongViewModelFactory = editSongViewModelFactory;
        _settingsViewModelFactory = settingsViewModelFactory;
        _editSongsViewModelFactory = editSongsViewModelFactory;
        _excludedSongsViewModelFactory = excludedSongsViewModelFactory;
        _exceptionViewModelFactory = exceptionViewModelFactory;
    }

    public async Task<NewResult<NewUnit>> LaunchSettings(Library library)
    {
        var viewModel = _settingsViewModelFactory.Create(library);
        var dialogAsync = await ShowDialogAsync(viewModel);
        return dialogAsync.CreateFromDialogResult();
    }

    public async Task<NewResult<NewUnit>> ShowEditSongs(List<Song> songs, Library library)
    {
        var viewModel = _editSongsViewModelFactory.Create(songs, library);

        var dialogAsync = await ShowDialogAsync(viewModel);
        return dialogAsync.CreateFromDialogResult();
    }

    public async Task<NewResult<NewUnit>> ShowEditSong(Song selectedSong, BitmapImage? albumArt, Library library)
    {
        var viewModel = _editSongViewModelFactory.Create(selectedSong, albumArt, library);

        var dialogAsync = await ShowDialogAsync(viewModel);
        return dialogAsync.CreateFromDialogResult();
    }

    public async Task<NewResult<NewUnit>> ShowException(Exception exception)
    {
        var viewModel = _exceptionViewModelFactory.Create(exception);
        var dialogAsync = await ShowDialogAsync(viewModel);
        return dialogAsync.CreateFromDialogResult();
    }

    public async Task<NewResult<NewUnit>> LaunchExcludedSongs(Library library)
    {
        var viewModel = _excludedSongsViewModelFactory.Create(library);

        var dialogAsync = await ShowDialogAsync(viewModel);
        return dialogAsync.CreateFromDialogResult();
    }
}