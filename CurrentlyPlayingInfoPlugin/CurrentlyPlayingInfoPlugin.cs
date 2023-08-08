using System.Text.Json;
using System.Timers;
using AIMP.SDK;
using AIMP.SDK.Player;
using WebSocketSharp;
using Timer = System.Timers.Timer;

namespace AIMP.CurrentlyPlayingInfoPlugin;

[AimpPlugin("Currently Playing Track Info Plugin", "Andruxxa7", "1.00")]
public class CurrentlyPlayingInfoPlugin : AimpPlugin
{
    public const string PluginName = "CurrentlyPlayingInfo";
    private IAimpServicePlayer _playerService;
    private string _path = "./config.json";
    private Timer _timer;
    private Settings _settings;
    private WebSocket _webSocket;

    public override void Initialize()
    {
        _playerService = Player.ServicePlayer;
        _settings = new Settings();
        if (File.Exists(_path))
        {
            try
            {
                _settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(_path),
                    JsonSerializerOptions.Default)!;
            }
            catch
            {
                /* ignored*/
            }
        }

        _webSocket = new WebSocket($"ws://{_settings.Host}:{_settings.Port}/aimp")
            { Origin = $"ws://{_settings.Host}:{_settings.Port}" };
        _timer = new Timer(_settings.Interval) { AutoReset = true };
        _timer.Elapsed += OnTimerOnElapsed;
        _timer.Start();
    }

    private async void OnTimerOnElapsed(object? sender, ElapsedEventArgs args)
    {
        await SendTrackInfoAndIsPlaying();
    }

    private Task SendTrackInfoAndIsPlaying()
    {
        var fileInfo = _playerService.CurrentFileInfo;

        var trackInfoMessage = new TrackInfoMessage
        {
            TrackTitle = fileInfo.Title,
            Artist = fileInfo.Artist,
            IsPlaying = _playerService.State == AimpPlayerState.Playing
        };

        {
            _webSocket.Connect();
            _webSocket.Send(JsonSerializer.Serialize(trackInfoMessage));
            _webSocket.Close(CloseStatusCode.Normal);
        }

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _timer.Close();
    }

    class TrackInfoMessage
    {
        public string TrackTitle { get; set; }
        public string Artist { get; set; }
        public bool IsPlaying { get; set; }
    }

    class Settings
    {
        public int Interval { get; set; } = 30000;
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 5543;
    }
}