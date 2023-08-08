using System.Text.Json;
using System.Timers;
using AIMP.SDK;
using AIMP.SDK.Player;
using WebSocketSharp;
using Timer = System.Timers.Timer;

#pragma warning disable CS8618

namespace AIMP.CurrentlyPlayingInfoPlugin
{
    [AimpPlugin("Currently Playing Track Info Plugin", "Andruxxa7", "1.00")]
    public class CurrentlyPlayingInfoPlugin : AimpPlugin
    {
        private IAimpServicePlayer _playerService;
        private string _path = "./config.json";
        private Timer _updateTimer;
        private WebSocket _webSocketClient;
        private PluginSettings _pluginSettings;

        public override void Initialize()
        {
            _playerService = Player.ServicePlayer;
            _pluginSettings = LoadSettings();

            _webSocketClient = new WebSocket($"ws://{_pluginSettings.Host}:{_pluginSettings.Port}/aimp")
            {
                Origin = $"ws://{_pluginSettings.Host}:{_pluginSettings.Port}"
            };

            _updateTimer = new Timer(_pluginSettings.Interval) { AutoReset = true };
            _updateTimer.Elapsed += OnTimerElapsed;
            _updateTimer.Start();
        }

        private async void OnTimerElapsed(object? sender, ElapsedEventArgs args)
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

            _webSocketClient.Connect();
            _webSocketClient.SendAsync(JsonSerializer.Serialize(trackInfoMessage), null);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _updateTimer.Close();
        }

        private PluginSettings LoadSettings()
        {
            if (File.Exists(_path))
            {
                try
                {
                    return JsonSerializer.Deserialize<PluginSettings>(File.ReadAllText(_path),
                        JsonSerializerOptions.Default) ?? new PluginSettings();
                }
                catch (Exception ex)
                {
                    File.WriteAllText($"Error while loading settings from your json: {ex.Message}", "./error.txt");
                }
            }

            return new PluginSettings();
        }

        private class TrackInfoMessage
        {
            public string TrackTitle { get; set; }
            public string Artist { get; set; }
            public bool IsPlaying { get; set; }
        }

        private class PluginSettings
        {
            public int Interval { get; set; } = 30000;
            public string Host { get; set; } = "127.0.0.1";
            public int Port { get; set; } = 5543;
        }
    }
}