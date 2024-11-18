﻿using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Client;

public class SettingsContainer
{
    public ISettings Settings { get; set; }

    public void Update(ISettings settings)
    {
        Settings = settings;
    }
}

public class AccessKeysContainer
{
    public string? GeniusToken { get; set; }
}