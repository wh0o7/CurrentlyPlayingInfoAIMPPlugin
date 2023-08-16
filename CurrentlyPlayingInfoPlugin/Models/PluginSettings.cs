using System.Text.Json.Serialization;

namespace AIMP.CurrentlyPlayingInfoPlugin.Models;

public class PluginSettings
{
    public int Interval { get; set; } = 30;
    public int WaitInterval { get; set; } = 60;
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 5543;
    public bool DebugMode { get; set; }
}