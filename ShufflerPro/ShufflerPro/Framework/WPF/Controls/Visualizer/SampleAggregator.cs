using NAudio.Dsp;

namespace ShufflerPro.Framework.WPF.Controls.Visualizer;

public class SampleAggregator
{
    private readonly int _binaryExponentitation;
    private readonly int _bufferSize;
    private readonly Complex[] _channelData;
    private int _channelDataPosition;

    public SampleAggregator(int bufferSize)
    {
        _bufferSize = bufferSize;
        _binaryExponentitation = (int)Math.Log(bufferSize, 2);
        _channelData = new Complex[bufferSize];
    }

    public float LeftMaxVolume { get; private set; }

    public float LeftMinVolume { get; private set; }

    public float RightMaxVolume { get; private set; }

    public float RightMinVolume { get; private set; }

    public void Clear()
    {
        LeftMaxVolume = float.MinValue;
        RightMaxVolume = float.MinValue;
        LeftMinVolume = float.MaxValue;
        RightMinVolume = float.MaxValue;
        _channelDataPosition = 0;
    }

    public void Add(float leftValue, float rightValue)
    {
        if (_channelDataPosition == 0)
        {
            LeftMaxVolume = float.MinValue;
            RightMaxVolume = float.MinValue;
            LeftMinVolume = float.MaxValue;
            RightMinVolume = float.MaxValue;
        }

        _channelData[_channelDataPosition].X = (leftValue + rightValue) / 2.0f;
        _channelData[_channelDataPosition].Y = 0;
        _channelDataPosition++;

        LeftMaxVolume = Math.Max(LeftMaxVolume, leftValue);
        LeftMinVolume = Math.Min(LeftMinVolume, leftValue);
        RightMaxVolume = Math.Max(RightMaxVolume, rightValue);
        RightMinVolume = Math.Min(RightMinVolume, rightValue);

        if (_channelDataPosition >= _channelData.Length) _channelDataPosition = 0;
    }

    public void GetFFTResults(float[] fftBuffer)
    {
        var channelDataClone = new Complex[_bufferSize];
        _channelData.CopyTo(channelDataClone, 0);
        FastFourierTransform.FFT(true, _binaryExponentitation, channelDataClone);
        for (var i = 0; i < channelDataClone.Length / 2; i++)
            fftBuffer[i] = (float)Math.Sqrt(channelDataClone[i].X * channelDataClone[i].X +
                                            channelDataClone[i].Y * channelDataClone[i].Y);
    }
}