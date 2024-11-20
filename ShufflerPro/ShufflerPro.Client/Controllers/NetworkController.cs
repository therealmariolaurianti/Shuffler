using System.Net.NetworkInformation;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ShufflerPro.Client.Controllers;

public class NetworkController
{
    private Timer? _networkUsageTimer;
    private long _previousBytesReceived;
    private long _previousBytesSent;

    public string? NetworkUsage { get; set; }

    public void Initialize()
    {
        _networkUsageTimer = new Timer(1000);

        _networkUsageTimer.Elapsed += OnNetworkUsageTimerElapsed;
        _networkUsageTimer.Start();
    }

    private void OnNetworkUsageTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        TrackNetworkUsage();
    }

    private void TrackNetworkUsage()
    {
        long bytesReceived = 0;
        long bytesSent = 0;

        // Get all network interfaces on the machine
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            if (networkInterface.OperationalStatus == OperationalStatus.Up) // Check if the interface is active
            {
                // Get the IPv4 statistics for the network interface
                var statistics = networkInterface.GetIPv4Statistics();
                bytesReceived += statistics.BytesReceived;
                bytesSent += statistics.BytesSent;
            }

        var receivedDifference = bytesReceived - _previousBytesReceived;

        // Update the previous bytes counters
        _previousBytesReceived = bytesReceived;
        _previousBytesSent = bytesSent;

        NetworkUsage = $"Net Usage: {receivedDifference / 1024} KB/s";
    }

    public void Stop()
    {
        _networkUsageTimer?.Stop();

        if (_networkUsageTimer is not null)
            _networkUsageTimer.Elapsed -= OnNetworkUsageTimerElapsed;

        _networkUsageTimer?.Dispose();
    }
}