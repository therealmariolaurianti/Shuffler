using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Controllers;

namespace ShufflerPro.Screens.AudioEqualizer;

public interface IAudioEqualizerViewModelFactory : IFactory
{
    AudioEqualizerViewModel Create(PlayerController playerController);
}