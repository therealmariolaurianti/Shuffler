namespace ShufflerPro.Client.AudioEqualizer;

public class EqualizerBandContainer : IEqualizerBandContainer
{
    public EqualizerBand[] Bands { get; internal set; } = [];

    public void Initialize()
    {
        Bands =
        [
            new EqualizerBand { Bandwidth = 0.8f, Frequency = 100, Gain = 0 },
            new EqualizerBand { Bandwidth = 0.8f, Frequency = 200, Gain = 0 },
            new EqualizerBand { Bandwidth = 0.8f, Frequency = 400, Gain = 0 },
            new EqualizerBand { Bandwidth = 0.8f, Frequency = 800, Gain = 0 },
            new EqualizerBand { Bandwidth = 0.8f, Frequency = 1200, Gain = 0 },
            new EqualizerBand { Bandwidth = 0.8f, Frequency = 2400, Gain = 0 },
            new EqualizerBand { Bandwidth = 0.8f, Frequency = 4800, Gain = 0 },
            new EqualizerBand { Bandwidth = 0.8f, Frequency = 9600, Gain = 0 }
        ];
    }
}