using NAudio.Dsp;
using NAudio.Wave;

namespace ShufflerPro.Client.AudioEqualizer;

/// <summary>
///     Basic example of a multi-band eq
///     uses the same settings for both channels in stereo audio
///     Call Update after you've updated the bands
///     Potentially to be added to NAudio in a future version
/// </summary>
public class Equalizer : ISampleProvider
{
    private readonly int bandCount;
    private readonly EqualizerBand[] bands;
    private readonly int channels;
    private readonly BiQuadFilter[,] filters;
    private readonly ISampleProvider sourceProvider;
    private bool updated;

    /// <summary>
    ///     Creates a new Equalizer
    /// </summary>
    public Equalizer(ISampleProvider sourceProvider, EqualizerBand[] bands)
    {
        this.sourceProvider = sourceProvider;
        this.bands = bands;
        channels = sourceProvider.WaveFormat.Channels;
        bandCount = bands.Length;
        filters = new BiQuadFilter[channels, bands.Length];
        CreateFilters();
    }

    /// <summary>
    ///     Gets the WaveFormat of this Sample Provider
    /// </summary>
    public WaveFormat WaveFormat => sourceProvider.WaveFormat;

    /// <summary>
    ///     Reads samples from this Sample Provider
    /// </summary>
    public int Read(float[] buffer, int offset, int count)
    {
        var samplesRead = sourceProvider.Read(buffer, offset, count);

        if (updated)
        {
            CreateFilters();
            updated = false;
        }

        for (var n = 0; n < samplesRead; n++)
        {
            var ch = n % channels;

            for (var band = 0; band < bandCount; band++)
                buffer[offset + n] = filters[ch, band].Transform(buffer[offset + n]);
        }

        return samplesRead;
    }

    private void CreateFilters()
    {
        for (var bandIndex = 0; bandIndex < bandCount; bandIndex++)
        {
            var band = bands[bandIndex];
            for (var n = 0; n < channels; n++)
                if (filters[n, bandIndex] == null)
                    filters[n, bandIndex] = BiQuadFilter.PeakingEQ(sourceProvider.WaveFormat.SampleRate, band.Frequency,
                        band.Bandwidth, band.Gain);
                else
                    filters[n, bandIndex].SetPeakingEq(sourceProvider.WaveFormat.SampleRate, band.Frequency,
                        band.Bandwidth, band.Gain);
        }
    }

    /// <summary>
    ///     Update the equalizer settings
    /// </summary>
    public void Update()
    {
        updated = true;
        CreateFilters();
    }
}