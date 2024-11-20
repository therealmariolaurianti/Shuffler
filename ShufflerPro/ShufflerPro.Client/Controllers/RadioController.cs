using NAudio.Wave;
using ShufflerPro.Client.Radio;
using ShufflerPro.Result;

namespace ShufflerPro.Client.Controllers;

public class RadioController
{
    private readonly IEnumerable<IRadioStation> _radioStations;

    public RadioController(IEnumerable<IRadioStation> radioStations)
    {
        _radioStations = radioStations;
    }

    public void StartStation(string url)
    {
        using (var mediaFoundationReader = new MediaFoundationReader(url))
        using (var wasapiOut = new WasapiOut())
        {
            wasapiOut.Init(mediaFoundationReader);
            wasapiOut.Play();
            
            //todo
            while (wasapiOut.PlaybackState == PlaybackState.Playing)
                Thread.Sleep(1000);
        }
    }

    public NewResult<List<IRadioStation>> GetStations()
    {
        return _radioStations.ToList();
    }
}