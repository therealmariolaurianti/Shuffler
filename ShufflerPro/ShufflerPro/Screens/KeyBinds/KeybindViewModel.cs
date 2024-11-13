using ShufflerPro.Client.Interfaces;
using ShufflerPro.Framework.WPF;

namespace ShufflerPro.Screens.KeyBinds;

public interface IKeybindViewModelFactory : IFactory
{
    KeybindViewModel Create();
}

public class KeybindViewModel : ViewModelBase
{
    
}