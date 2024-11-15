using JetBrains.Annotations;
using ShufflerPro.Client.AudioEqualizer;
using ShufflerPro.Framework.WPF;

namespace ShufflerPro.Screens.AudioEqualizer;

public class AudioEqualizerViewModel : ViewModelBase
{
    private readonly IEqualizerBandContainer _equalizerBandContainer;

    public AudioEqualizerViewModel(IEqualizerBandContainer equalizerBandContainer)
    {
        _equalizerBandContainer = equalizerBandContainer;
        Bands = equalizerBandContainer.Bands;
    }

    public EqualizerBand[] Bands { get; }

    public float MinimumGain => -30;
    public float MaximumGain => 30;

    public float Band1
    {
        get => Bands[0].Gain;
        set
        {
            Bands[0].Gain = value;
            NotifyOfPropertyChange();
        }
    }

    public float Band2
    {
        get => Bands[1].Gain;
        set
        {
            Bands[1].Gain = value;
            NotifyOfPropertyChange();
        }
    }

    public float Band3
    {
        get => Bands[2].Gain;
        set
        {
            Bands[2].Gain = value;
            NotifyOfPropertyChange();
        }
    }

    public float Band4
    {
        get => Bands[3].Gain;
        set
        {
            Bands[3].Gain = value;
            NotifyOfPropertyChange();
        }
    }

    public float Band5
    {
        get => Bands[4].Gain;
        set
        {
            Bands[4].Gain = value;
            NotifyOfPropertyChange();
        }
    }

    public float Band6
    {
        get => Bands[5].Gain;
        set
        {
            Bands[5].Gain = value;
            NotifyOfPropertyChange();
        }
    }


    public float Band7
    {
        get => Bands[6].Gain;
        set
        {
            Bands[6].Gain = value;
            NotifyOfPropertyChange();
        }
    }

    public float Band8
    {
        get => Bands[7].Gain;
        set
        {
            Bands[7].Gain = value;
            NotifyOfPropertyChange();
        }
    }

    [UsedImplicitly]
    public void ResetBands()
    {
        _equalizerBandContainer.Initialize();
    }
}