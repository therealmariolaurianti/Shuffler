using NAudio.Wave;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Screens.AudioEqualizer;

public interface IAudioEqualizerViewModelFactory : IFactory
{
    AudioEqualizerViewModel Create(ISampleProvider sampleProvider);
}