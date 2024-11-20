using NAudio.Wave;
using ShufflerPro.Result;
using ShufflerPro.Web.Radio;

namespace ShufflerPro.Web;

public class RadioController
{
    private readonly IEnumerable<IRadioStation> _radioStations;

    public RadioController(IEnumerable<IRadioStation> radioStations)
    {
        _radioStations = radioStations;
    }

    public void StartStation(IRadioStation radioStation)
    {
        using (var mediaFoundationReader = new MediaFoundationReader(radioStation.Url))
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