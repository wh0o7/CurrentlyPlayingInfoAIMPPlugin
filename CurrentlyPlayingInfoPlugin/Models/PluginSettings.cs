namespace AIMP.CurrentlyPlayingInfoPlugin.Models;

public class PluginSettings
{
    private int _interval = 30;
    private int _waitInterval = 60;

    public int Interval
    {
        get => _interval;
        set => _interval = value is >= 10 and <= 300 ? value : 30;
    }

    public int WaitInterval
    {
        get => _waitInterval;
        set => _waitInterval = value is >= 20 and <= 600 ? value : 60;
    }

    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 5543;
    public bool DebugMode { get; set; }
}