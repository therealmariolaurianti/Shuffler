using ShufflerPro.Client.Entities;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Screens.Setting;

public interface ISettingsViewModelFactory : IFactory
{
    SettingsViewModel Create(Library library);
}