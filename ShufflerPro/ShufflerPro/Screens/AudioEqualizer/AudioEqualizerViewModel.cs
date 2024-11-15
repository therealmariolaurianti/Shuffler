using NAudio.Wave;
using ShufflerPro.Client.Interfaces;
using ShufflerPro.Framework.WPF;
using ShufflerPro.Framework.WPF.Controls;

namespace ShufflerPro.Screens.AudioEqualizer;

public interface IAudioEqualizerViewModelFactory : IFactory
{
    AudioEqualizerViewModel Create(ISampleProvider sampleProvider);
}

public class AudioEqualizerViewModel : ViewModelBase
{
    public AudioEqualizerViewModel(ISampleProvider sampleProvider)
    {
        var equalizer = new Equalizer(sampleProvider, [new EqualizerBand()]);
    }
}