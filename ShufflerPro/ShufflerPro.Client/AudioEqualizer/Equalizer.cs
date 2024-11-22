using System;
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
    private readonly EqualizerBand[] _bands;
    private readonly IWaveProvider _sampleProvider;
    private readonly int bandCount;
    private readonly int channels;

    private readonly BiQuadFilter[,] filters;

    //private readonly ISampleProvider _sourceProvider;
    private bool updated;

    /// <summary>
    ///     Creates a new Equalizer
    /// </summary>
    public Equalizer(IWaveProvider sampleProvider, EqualizerBand[] bands)
    {
        _sampleProvider = sampleProvider;
        WaveFormat = sampleProvider.WaveFormat;
        _bands = bands;
        channels = WaveFormat.Channels;
        bandCount = bands.Length;
        filters = new BiQuadFilter[channels, bands.Length];
        CreateFilters();
    }

    /// <summary>
    ///     Gets the WaveFormat of this Sample Provider
    /// </summary>
    public WaveFormat WaveFormat { get; }

    /// <summary>
    ///     Reads samples from this Sample Provider
    /// </summary>
    public int Read(float[] buffer, int offset, int count)
    {
        var byteBuffer = new byte[buffer.Length * 4];
        var bytesRead = _sampleProvider.Read(byteBuffer, offset * 4, count * 4);
        var samplesRead = bytesRead / 4;
        Buffer.BlockCopy(byteBuffer, 0, buffer, offset * 4, bytesRead);

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
            var band = _bands[bandIndex];
            for (var n = 0; n < channels; n++)
                if (filters[n, bandIndex] == null)
                    filters[n, bandIndex] = BiQuadFilter.PeakingEQ(WaveFormat.SampleRate, band.Frequency,
                        band.Bandwidth, band.Gain);
                else
                    filters[n, bandIndex].SetPeakingEq(WaveFormat.SampleRate, band.Frequency,
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