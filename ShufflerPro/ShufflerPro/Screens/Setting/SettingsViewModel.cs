using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Framework;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;

namespace ShufflerPro.Screens.Setting;

public class SettingsViewModel : ViewModelBase
{
    private readonly ItemTracker<Settings> _itemTracker;
    private readonly Library _library;
    private readonly ISettings _settings;
    private readonly SettingsController _settingsController;
    private readonly ShufflerWindowManager _windowManager;
    private bool _canSave;
    private bool _saving;

    public SettingsViewModel(
        Library library,
        ISettings settings,
        ItemTracker<Settings> itemTracker,
        SettingsController settingsController,
        ShufflerWindowManager windowManager)
    {
        _library = library;
        _settings = settings;
        _itemTracker = itemTracker;
        _settingsController = settingsController;
        _windowManager = windowManager;

        _itemTracker.Attach((Settings)settings);
    }

    public bool IsDarkModeEnabled
    {
        get => _settings.IsDarkModeEnabled;
        set
        {
            if (value == _settings.IsDarkModeEnabled) return;
            _settings.IsDarkModeEnabled = value;
            NotifyOfPropertyChange();
            ThemeManager.ChangeTheme(ThemeId, value);
        }
    }

    public Guid ThemeId
    {
        get => _settings.ThemeId;
        set
        {
            if (value.Equals(_settings.ThemeId)) return;
            _settings.ThemeId = value;
            NotifyOfPropertyChange();
            ThemeManager.ChangeTheme(value, IsDarkModeEnabled);
        }
    }

    public bool CanSave
    {
        get => _canSave;
        set
        {
            if (value == _canSave) return;
            _canSave = value;
            NotifyOfPropertyChange();
        }
    }

    public override void NotifyOfPropertyChange([CallerMemberName] string? propertyName = null)
    {
        CanSave = _itemTracker.IsDirty;
        base.NotifyOfPropertyChange(propertyName);
    }

    [UsedImplicitly]
    public void Save()
    {
        _saving = true;
        RunAsync(async () => await _settingsController
            .Update(_itemTracker)
            .IfSuccessAsync(async _ => await TryCloseAsync(true)));
    }

    [UsedImplicitly]
    public void Cancel()
    {
        _itemTracker.Revert();
        TryCloseAsync(false);
    }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = new())
    {
        if (_itemTracker.IsDirty && !_saving)
            _itemTracker.Revert();
        return base.CanCloseAsync(cancellationToken);
    }

    [UsedImplicitly]
    public void LaunchExcludedSongs()
    {
        RunAsync(async () => await _windowManager.LaunchExcludedSongs(_library));
    }
}