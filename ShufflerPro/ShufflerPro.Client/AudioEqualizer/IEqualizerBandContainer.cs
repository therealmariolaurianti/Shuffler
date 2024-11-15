namespace ShufflerPro.Client.AudioEqualizer;

public interface IEqualizerBandContainer
{
    void Initialize();
    EqualizerBand[] Bands { get; }
}