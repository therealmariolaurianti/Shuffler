//taken and refactored from: Jacob Johnston - https://github.com/jacobjohnston/wpfsvl

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using NAudio.Wave;
using WPFSoundVisualizationLib;

namespace ShufflerPro.Framework.WPF.Controls.Visualizer;

public class VisualizerEngine : ISpectrumPlayer, IWaveformPlayer
{
    private const int _waveformCompressedPointCount = 2000;
    private const int _repeatThreshold = 200;
    private const int _fftDataSize = (int)FFTDataSize.FFT2048;

    private static VisualizerEngine? _instance;
    private readonly DispatcherTimer _positionTimer = new(DispatcherPriority.ApplicationIdle);
    private readonly BackgroundWorker _waveformGenerateWorker = new();
    private WaveStream? _activeStream;
    private double _channelLength;
    private WaveChannel32? _inputStream;

    private bool _inRepeatSet;
    private bool _isPlaying;
    private bool _isStreaming;
    private string _pendingWaveformPath;
    private TimeSpan _repeatStart;
    private TimeSpan _repeatStop;
    private SampleAggregator _sampleAggregator;
    private SampleAggregator _waveformAggregator;
    private float[] _waveformData;
    private double channelPosition;
    private bool inChannelSet;
    private bool inChannelTimerUpdate;

    private VisualizerEngine()
    {
        _positionTimer.Interval = TimeSpan.FromMilliseconds(50);
        _positionTimer.Tick += positionTimer_Tick;

        _waveformGenerateWorker.DoWork += waveformGenerateWorker_DoWork;
        _waveformGenerateWorker.RunWorkerCompleted += waveformGenerateWorker_RunWorkerCompleted;
        _waveformGenerateWorker.WorkerSupportsCancellation = true;
    }

    public static VisualizerEngine Instance => _instance ??= new VisualizerEngine();

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
        get => _repeatStart;
        set
        {
            if (!_inRepeatSet)
            {
                _inRepeatSet = true;
                var oldValue = _repeatStart;
                _repeatStart = value;
                if (oldValue != _repeatStart)
                    NotifyPropertyChanged();
                _inRepeatSet = false;
            }
        }
    }

    public TimeSpan SelectionEnd
    {
        get => _repeatStop;
        set
        {
            if (!inChannelSet)
            {
                _inRepeatSet = true;
                var oldValue = _repeatStop;
                _repeatStop = value;
                if (oldValue != _repeatStop)
                    NotifyPropertyChanged();
                _inRepeatSet = false;
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
        get => _channelLength;
        protected set
        {
            if (_channelLength == value)
                return;
            _channelLength = value;
            NotifyPropertyChanged();
        }
    }

    public double ChannelPosition
    {
        get => channelPosition;
        set => SetChannel(value);
    }

    private void SetChannel(double value)
    {
        if (!inChannelSet)
        {
            if (_activeStream is null)
                return;

            inChannelSet = true; // Avoid recursion

            var oldValue = channelPosition;
            var position = Math.Max(0, Math.Min(value, ChannelLength));

            if (!inChannelTimerUpdate && !_isStreaming)
                _activeStream.Position = (long)(position / _activeStream.TotalTime.TotalSeconds * _activeStream.Length);
            else if (_isStreaming)
                _activeStream.Position = 0;

            channelPosition = position;
            if (oldValue != channelPosition)
                NotifyPropertyChanged();

            inChannelSet = false;
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
                _pendingWaveformPath = path;
                _waveformGenerateWorker.CancelAsync();
                return;
            case false when _waveformCompressedPointCount != 0:
                _waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(_waveformCompressedPointCount,
                    path, _isStreaming));
                break;
        }
    }

    private void waveformGenerateWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Cancelled)
            if (!_waveformGenerateWorker.IsBusy && _waveformCompressedPointCount != 0)
                _waveformGenerateWorker.RunWorkerAsync(
                    new WaveformGenerationParams(_waveformCompressedPointCount,
                        _pendingWaveformPath, _isStreaming));
    }

    private void waveformGenerateWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        if (e.Argument is not WaveformGenerationParams waveformParams)
            return;

        WaveStream reader;
        if (waveformParams.IsStreaming)
            reader = new MediaFoundationReader(waveformParams.Path);
        else
            reader = new AudioFileReader(waveformParams.Path);

        var waveformInputStream = new WaveChannel32(reader);
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

        reader.Close();
        reader.Dispose();
    }

    public void Stop()
    {
        IsPlaying = false;
        SelectionBegin = TimeSpan.Zero;
        SelectionEnd = TimeSpan.Zero;
    }

    public void Reset()
    {
        Stop();

        _inputStream?.Close();
        _inputStream = null;
        _activeStream = null;
    }

    public IWaveProvider StartVisualizer(WaveStream activeStream, string path,
        bool isStreaming)
    {
        Reset();

        _activeStream = activeStream;
        _isStreaming = isStreaming;
        
        ChannelPosition = 0;

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

        return _inputStream!;
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
        ChannelPosition = _activeStream?.Position / (double?)_activeStream?.Length *
            _activeStream?.TotalTime.TotalSeconds ?? 0;
        inChannelTimerUpdate = false;
    }
}

internal class WaveformGenerationParams(int points, string path, bool isStreaming)
{
    public int Points { get; } = points;
    public string Path { get; } = path;
    public bool IsStreaming { get; } = isStreaming;
}