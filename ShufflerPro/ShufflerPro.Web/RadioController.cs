using NAudio.Wave;

namespace ShufflerPro.Web;

public class RadioController
{
    public void StartStation(IRadioStation radioStation)
    {
        using (var mediaFoundationReader = new MediaFoundationReader(radioStation.Url))
        using (var wasapiOut = new WasapiOut())
        {
            wasapiOut.Init(mediaFoundationReader);
            wasapiOut.Play();
            
            while (wasapiOut.PlaybackState == PlaybackState.Playing)
                Thread.Sleep(1000);
        }
    }
}

public interface IRadioStation
{
    public string Name { get; }
    public string Url { get; }
}

public class ChillHopRadioStation : IRadioStation
{
    public string Name => "ChillHop";
    public string Url => "https://streams.fluxfm.de/Chillhop/mp3-128/streams.fluxfm.de/";
}

public class LofiRadioStation : IRadioStation
{
    public string Name => "Lofi";
    public string Url => "https://streams.fluxfm.de/Chillhop/aac-64/streams.fluxfm.de/";
}