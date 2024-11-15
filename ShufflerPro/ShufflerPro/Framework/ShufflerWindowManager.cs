using System.Windows.Media.Imaging;
using Caliburn.Micro;
using NAudio.Wave;
using ShufflerPro.Client.Entities;
using ShufflerPro.Result;
using ShufflerPro.Screens.AudioEqualizer;
using ShufflerPro.Screens.EditSong.Multiple;
using ShufflerPro.Screens.EditSong.Single;
using ShufflerPro.Screens.Exceptions;
using ShufflerPro.Screens.ExcludedSongs;
using ShufflerPro.Screens.Setting;

namespace ShufflerPro.Framework;

public class ShufflerWindowManager : WindowManager
{
    private readonly IAudioEqualizerViewModelFactory _audioEqualizerViewModelFactory;
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
        IExceptionViewModelFactory exceptionViewModelFactory,
        IAudioEqualizerViewModelFactory audioEqualizerViewModelFactory)
    {
        _editSongViewModelFactory = editSongViewModelFactory;
        _settingsViewModelFactory = settingsViewModelFactory;
        _editSongsViewModelFactory = editSongsViewModelFactory;
        _excludedSongsViewModelFactory = excludedSongsViewModelFactory;
        _exceptionViewModelFactory = exceptionViewModelFactory;
        _audioEqualizerViewModelFactory = audioEqualizerViewModelFactory;
    }

    public async Task<NewResult<NewUnit>> ShowSettings(Library library)
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

    public async Task<NewResult<NewUnit>> ShowExcludedSongs(Library library)
    {
        var viewModel = _excludedSongsViewModelFactory.Create(library);

        var dialogAsync = await ShowDialogAsync(viewModel);
        return dialogAsync.CreateFromDialogResult();
    }

    public async Task<NewResult<NewUnit>> ShowAudioEqualizer(ISampleProvider sampleProvider)
    {
        var viewModel = _audioEqualizerViewModelFactory.Create(sampleProvider);

        var dialogAsync = await ShowDialogAsync(viewModel);
        return dialogAsync.CreateFromDialogResult();
    }
}