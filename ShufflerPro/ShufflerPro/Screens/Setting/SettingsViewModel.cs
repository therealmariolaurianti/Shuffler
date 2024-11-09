﻿using ShufflerPro.Client;
using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Result;

namespace ShufflerPro.Screens.Setting;

public interface ISettingsViewModelFactory : IFactory
{
    SettingsViewModel Create();
}

public class SettingsViewModel : ViewModelBase
{
    private readonly ItemTracker<Settings> _itemTracker;
    private readonly ISettings _settings;
    private readonly SettingsController _settingsController;
    private bool _saving;

    public SettingsViewModel(ISettings settings, ItemTracker<Settings> itemTracker,
        SettingsController settingsController)
    {
        _settings = settings;
        _itemTracker = itemTracker;
        _settingsController = settingsController;

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

    public void Save()
    {
        _saving = true;
        RunAsync(async () => await _settingsController
            .Update(_itemTracker)
            .IfSuccessAsync(async _ => await TryCloseAsync(true)));
    }

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
}