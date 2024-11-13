using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ShufflerPro.Client.Entities;
using ShufflerPro.Result;
using ShufflerPro.Screens.EditSong.Multiple;
using ShufflerPro.Screens.EditSong.Single;
using ShufflerPro.Screens.ExcludedSongs;
using ShufflerPro.Screens.KeyBinds;
using ShufflerPro.Screens.Setting;

namespace ShufflerPro.Framework;

public class ShufflerWindowManager : WindowManager
{
    private readonly IEditSongsViewModelFactory _editSongsViewModelFactory;
    private readonly IEditSongViewModelFactory _editSongViewModelFactory;
    private readonly IExcludedSongsViewModelFactory _excludedSongsViewModelFactory;
    private readonly IKeybindViewModelFactory _keybindViewModelFactory;
    private readonly ISettingsViewModelFactory _settingsViewModelFactory;

    public ShufflerWindowManager(
        IEditSongViewModelFactory editSongViewModelFactory,
        ISettingsViewModelFactory settingsViewModelFactory,
        IEditSongsViewModelFactory editSongsViewModelFactory,
        IExcludedSongsViewModelFactory excludedSongsViewModelFactory,
        IKeybindViewModelFactory keybindViewModelFactory)
    {
        _editSongViewModelFactory = editSongViewModelFactory;
        _settingsViewModelFactory = settingsViewModelFactory;
        _editSongsViewModelFactory = editSongsViewModelFactory;
        _excludedSongsViewModelFactory = excludedSongsViewModelFactory;
        _keybindViewModelFactory = keybindViewModelFactory;
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

    public void ShowMessageBox(Exception exception)
    {
        MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public async Task<NewResult<NewUnit>> LaunchExcludedSongs(Library library)
    {
        var viewModel = _excludedSongsViewModelFactory.Create(library);

        var dialogAsync = await ShowDialogAsync(viewModel);
        return dialogAsync.CreateFromDialogResult();
    }

    public async Task<NewResult<NewUnit>> LaunchKeyBinds()
    {
        var viewModel = _keybindViewModelFactory.Create();

        var dialogAsync = await ShowDialogAsync(viewModel);
        return dialogAsync.CreateFromDialogResult();
    }
}