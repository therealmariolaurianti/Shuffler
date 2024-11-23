using NAudio.Wave;
using ShufflerPro.Client.Radio;
using ShufflerPro.Framework.WPF.Controls.Visualizer;
using ShufflerPro.Result;

namespace ShufflerPro.Controllers;

public class RadioController(
    IEnumerable<IRadioStation> radioStations)
{
    private WasapiOut? _wasapiOut;
    public bool IsPlaying => _wasapiOut?.PlaybackState == PlaybackState.Playing;

    public void StartStation(string url)
    {
        StopStation();

        var mediaFoundationReader = new MediaFoundationReader(url);

         var inputStream = VisualizerEngine.Instance
             .StartVisualizer(mediaFoundationReader, url, true);

        _wasapiOut ??= new WasapiOut();

        _wasapiOut.Init(inputStream);
        _wasapiOut.Play();
    }

    public NewResult<List<IRadioStation>> GetStations()
    {
        return radioStations.ToList();
    }

    public void StopStation()
    {
        _wasapiOut?.Stop();
        _wasapiOut?.Dispose();
        _wasapiOut = null;
    }
}