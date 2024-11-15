using ShufflerPro.Client.Controllers;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Screens.AudioEqualizer;

public interface IAudioEqualizerViewModelFactory : IFactory
{
    AudioEqualizerViewModel Create(PlayerController playerController);
}