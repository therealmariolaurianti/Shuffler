using NAudio.Wave;
using ShufflerPro.Client.Radio;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class RadioController(
    IEnumerable<IRadioStation> radioStations,
    NetworkController networkController)
{
    private WasapiOut? _wasapiOut;
    public bool IsPlaying => _wasapiOut?.PlaybackState == PlaybackState.Playing;

    public string? NetworkUsage => IsPlaying ? networkController.NetworkUsage : null;

    public void StartStation(string url)
    {
        StopStation();

        using (var mediaFoundationReader = new MediaFoundationReader(url))
        {
            _wasapiOut = new WasapiOut();

            _wasapiOut.Init(mediaFoundationReader);
            _wasapiOut.Play();

            networkController.Initialize();
        }
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

        networkController.Stop();
    }
}