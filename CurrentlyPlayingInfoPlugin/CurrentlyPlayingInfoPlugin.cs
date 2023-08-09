using System.Text.Json;
using System.Timers;
using AIMP.CurrentlyPlayingInfoPlugin.Models;
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
        private bool IsWaitMode;
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
            OnTimerElapsed(null, null);
            _updateTimer.Elapsed += OnTimerElapsed;
            _updateTimer.Start();
        }

        private async void OnTimerElapsed(object? sender, ElapsedEventArgs args)
        {
            try
            {
                _webSocketClient.Connect();
                if (_webSocketClient.Ping())
                {
                    await SendTrackInfoAndIsPlaying();
                    if (IsWaitMode) await DisableWaitMode();
                    return;
                }
            }
            catch
            {
                if (!IsWaitMode) await EnableWaitMode();
            }

            if (IsWaitMode)
            {
                await WaitModeTask();
                return;
            }

            await EnableWaitMode();
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

        private Task WaitModeTask()
        {
            Console.WriteLine("Waiting for connecting.");
            return Task.CompletedTask;
        }

        private Task EnableWaitMode()
        {
            IsWaitMode = true;
            _updateTimer.Interval = _pluginSettings.WaitInterval;
            return Task.CompletedTask;
        }


        private Task DisableWaitMode()
        {
            IsWaitMode = false;
            _updateTimer.Interval = _pluginSettings.Interval;
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _updateTimer.Close();
            _webSocketClient.Close();
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
    }
}