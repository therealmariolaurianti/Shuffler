using NAudio.Wave;
using ShufflerPro.Client.AudioEqualizer;
using ShufflerPro.Client.Radio;
using ShufflerPro.Framework.WPF.Controls.Visualizer;
using ShufflerPro.Result;

namespace ShufflerPro.Controllers;

public class RadioController(
    IEnumerable<IRadioStation> radioStations,
    IEqualizerBandContainer equalizerBandContainer)
{
    private Equalizer? _equalizer;
    private WasapiOut? _wasapiOut;
    public bool IsPlaying => _wasapiOut?.PlaybackState == PlaybackState.Playing;

    public void StartStation(string url)
    {
        StopStation();

        var mediaFoundationReader = new MediaFoundationReader(url);

        var inputStream = VisualizerEngine.Instance
            .StartVisualizer(mediaFoundationReader, url, true);

        _equalizer = new Equalizer(inputStream, equalizerBandContainer.Bands);

        _wasapiOut ??= new WasapiOut();

        _wasapiOut.Init(_equalizer);
        _wasapiOut.Play();
        
        //TODO 
        //VisualizerEngine.Instance.IsPlaying = true;
    }

    public NewResult<List<IRadioStation>> GetStations()
    {
        return radioStations.ToList();
    }

    public void StopStation()
    {
        _wasapiOut?.Stop();
        _wasapiOut?.Dispose();

        _equalizer = null;
        _wasapiOut = null;
    }
}