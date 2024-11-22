using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using NAudio.Wave;
using WPFSoundVisualizationLib;

namespace ShufflerPro.Screens.Shell.Visualizer;

public class NAudioEngine : ISpectrumPlayer, IWaveformPlayer
{
    private const int _waveformCompressedPointCount = 2000;
    private const int _repeatThreshold = 200;

    private static NAudioEngine? _instance;
    private readonly int _fftDataSize = (int)FFTDataSize.FFT2048;
    private readonly DispatcherTimer _positionTimer = new(DispatcherPriority.ApplicationIdle);
    private readonly BackgroundWorker _waveformGenerateWorker = new();
    private WaveStream? _activeStream;
    private WaveChannel32 _inputStream;
    private bool _isPlaying;
    private SampleAggregator _sampleAggregator;
    private SampleAggregator _waveformAggregator;
    private float[] _waveformData;

    private double channelLength;
    private double channelPosition;
    private bool inChannelSet;
    private bool inChannelTimerUpdate;

    private bool inRepeatSet;
    private string pendingWaveformPath;
    private TimeSpan repeatStart;
    private TimeSpan repeatStop;

    private NAudioEngine()
    {
        _positionTimer.Interval = TimeSpan.FromMilliseconds(50);
        _positionTimer.Tick += positionTimer_Tick;

        _waveformGenerateWorker.DoWork += waveformGenerateWorker_DoWork;
        _waveformGenerateWorker.RunWorkerCompleted += waveformGenerateWorker_RunWorkerCompleted;
        _waveformGenerateWorker.WorkerSupportsCancellation = true;
    }

    public static NAudioEngine Instance => _instance ??= new NAudioEngine();

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool GetFFTData(float[] fftDataBuffer)
    {
        _sampleAggregator.GetFFTResults(fftDataBuffer);
        return IsPlaying;
    }

    public int GetFFTFrequencyIndex(int frequency)
    {
        double maxFrequency;
        if (_activeStream != null)
            maxFrequency = _activeStream.WaveFormat.SampleRate / 2.0d;
        else
            maxFrequency = 22050;
        return (int)(frequency / maxFrequency * (_fftDataSize / 2));
    }


    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (_isPlaying == value)
                return;
            _isPlaying = value;
            NotifyPropertyChanged();

            _positionTimer.IsEnabled = value;
        }
    }

    public TimeSpan SelectionBegin
    {
        get => repeatStart;
        set
        {
            if (!inRepeatSet)
            {
                inRepeatSet = true;
                var oldValue = repeatStart;
                repeatStart = value;
                if (oldValue != repeatStart)
                    NotifyPropertyChanged();
                inRepeatSet = false;
            }
        }
    }

    public TimeSpan SelectionEnd
    {
        get => repeatStop;
        set
        {
            if (!inChannelSet)
            {
                inRepeatSet = true;
                var oldValue = repeatStop;
                repeatStop = value;
                if (oldValue != repeatStop)
                    NotifyPropertyChanged();
                inRepeatSet = false;
            }
        }
    }

    public float[] WaveformData
    {
        get => _waveformData;
        protected set
        {
            var oldValue = _waveformData;
            _waveformData = value;
            if (oldValue != _waveformData)
                NotifyPropertyChanged();
        }
    }

    public double ChannelLength
    {
        get => channelLength;
        protected set
        {
            var oldValue = channelLength;
            channelLength = value;
            if (oldValue != channelLength)
                NotifyPropertyChanged();
        }
    }

    public double ChannelPosition
    {
        get => channelPosition;
        set
        {
            if (!inChannelSet)
            {
                inChannelSet = true; // Avoid recursion
                var oldValue = channelPosition;
                var position = Math.Max(0, Math.Min(value, ChannelLength));
                if (!inChannelTimerUpdate && _activeStream != null)
                    _activeStream.Position =
                        (long)(position / _activeStream.TotalTime.TotalSeconds * _activeStream.Length);
                channelPosition = position;
                if (oldValue != channelPosition)
                    NotifyPropertyChanged();
                inChannelSet = false;
            }
        }
    }

    private void NotifyPropertyChanged([CallerMemberName] string? info = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
    }

    private void GenerateWaveformData(string path)
    {
        switch (_waveformGenerateWorker.IsBusy)
        {
            case true:
                pendingWaveformPath = path;
                _waveformGenerateWorker.CancelAsync();
                return;
            case false when _waveformCompressedPointCount != 0:
                _waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(_waveformCompressedPointCount,
                    path));
                break;
        }
    }

    private void waveformGenerateWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Cancelled)
            if (!_waveformGenerateWorker.IsBusy && _waveformCompressedPointCount != 0)
                _waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(_waveformCompressedPointCount,
                    pendingWaveformPath));
    }

    private void waveformGenerateWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        if (!(e.Argument is WaveformGenerationParams waveformParams))
            return;

        var waveformMp3Stream = new Mp3FileReader(waveformParams.Path);
        var waveformInputStream = new WaveChannel32(waveformMp3Stream);
        waveformInputStream.Sample += waveStream_Sample;

        var frameLength = _fftDataSize;
        var frameCount = (int)(waveformInputStream.Length / (double)frameLength);
        var waveformLength = frameCount * 2;
        var readBuffer = new byte[frameLength];
        _waveformAggregator = new SampleAggregator(frameLength);

        var maxLeftPointLevel = float.MinValue;
        var maxRightPointLevel = float.MinValue;
        var currentPointIndex = 0;
        var waveformCompressedPoints = new float[waveformParams.Points];


        var waveMaxPointIndexes = new List<int>();

        for (var i = 1; i <= waveformParams.Points; i++)
            waveMaxPointIndexes.Add((int)Math.Round(waveformLength * (i / (double)waveformParams.Points), 0));
        var readCount = 0;
        while (currentPointIndex * 2 < waveformParams.Points)
        {
            _ = waveformInputStream.Read(readBuffer, 0, readBuffer.Length);

            if (_waveformAggregator.LeftMaxVolume > maxLeftPointLevel)
                maxLeftPointLevel = _waveformAggregator.LeftMaxVolume;
            if (_waveformAggregator.RightMaxVolume > maxRightPointLevel)
                maxRightPointLevel = _waveformAggregator.RightMaxVolume;

            if (readCount > waveMaxPointIndexes[currentPointIndex])
            {
                waveformCompressedPoints[currentPointIndex * 2] = maxLeftPointLevel;
                waveformCompressedPoints[currentPointIndex * 2 + 1] = maxRightPointLevel;
                maxLeftPointLevel = float.MinValue;
                maxRightPointLevel = float.MinValue;
                currentPointIndex++;
            }

            if (readCount % 3000 == 0)
            {
                var clonedData = (float[])waveformCompressedPoints.Clone();
                Application.Current.Dispatcher.Invoke(() => { WaveformData = clonedData; });
            }

            if (_waveformGenerateWorker.CancellationPending)
            {
                e.Cancel = true;
                break;
            }

            readCount++;
        }

        var finalClonedData = (float[])waveformCompressedPoints.Clone();
        Application.Current.Dispatcher.Invoke(() => { WaveformData = finalClonedData; });

        waveformInputStream.Close();
        waveformInputStream.Dispose();

        waveformMp3Stream.Close();
        waveformMp3Stream.Dispose();
    }

    public IWaveProvider StartVisualizer(AudioFileReader activeStream, string path)
    {
        if (_activeStream != null)
        {
            SelectionBegin = TimeSpan.Zero;
            SelectionEnd = TimeSpan.Zero;
            ChannelPosition = 0;
        }

        _activeStream = activeStream;

        try
        {
            _sampleAggregator = new SampleAggregator(_fftDataSize);
            _inputStream = new WaveChannel32(_activeStream);

            _inputStream.Sample += inputStream_Sample;

            ChannelLength = _inputStream.TotalTime.TotalSeconds;

            GenerateWaveformData(path);
        }
        catch
        {
            _activeStream = null;
        }

        return _inputStream;
    }

    private void inputStream_Sample(object? sender, SampleEventArgs e)
    {
        _sampleAggregator.Add(e.Left, e.Right);
        var repeatStartPosition =
            (long)(SelectionBegin.TotalSeconds / _activeStream!.TotalTime.TotalSeconds * _activeStream.Length);
        var repeatStopPosition =
            (long)(SelectionEnd.TotalSeconds / _activeStream.TotalTime.TotalSeconds * _activeStream.Length);
        if (SelectionEnd - SelectionBegin >= TimeSpan.FromMilliseconds(_repeatThreshold) &&
            _activeStream.Position >= repeatStopPosition)
        {
            _sampleAggregator.Clear();
            _activeStream.Position = repeatStartPosition;
        }
    }

    private void waveStream_Sample(object? sender, SampleEventArgs e)
    {
        _waveformAggregator.Add(e.Left, e.Right);
    }

    private void positionTimer_Tick(object? sender, EventArgs e)
    {
        if (_activeStream is null)
            return;

        inChannelTimerUpdate = true;
        ChannelPosition = _activeStream.Position / (double)_activeStream.Length * _activeStream.TotalTime.TotalSeconds;
        inChannelTimerUpdate = false;
    }

    private class WaveformGenerationParams
    {
        public WaveformGenerationParams(int points, string path)
        {
            Points = points;
            Path = path;
        }

        public int Points { get; }
        public string Path { get; }
    }
}