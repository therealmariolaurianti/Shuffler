using NAudio.Wave;
using ShufflerPro.Client.Radio;
using ShufflerPro.Framework;
using ShufflerPro.Result;

namespace ShufflerPro.Controllers;

public class RadioController(IEnumerable<IRadioStation> radioStations, ShufflerWindowManager windowManager)
{
    private WasapiOut? _wasapiOut;
    public bool IsPlaying => _wasapiOut?.PlaybackState == PlaybackState.Playing;

    public void StartStation(string url)
    {
        try
        {
            StopStation();

            using (var mediaFoundationReader = new MediaFoundationReader(url))
            {
                _wasapiOut = new WasapiOut();

                _wasapiOut.Init(mediaFoundationReader);
                _wasapiOut.Play();
            }
        }
        catch (Exception e)
        {
            windowManager.ShowException(e);
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
    }
}