using System.Text.Json;
using System.Timers;
using AIMP.CurrentlyPlayingInfoPlugin.Models;
using AIMP.CurrentlyPlayingInfoPlugin.Services;
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
        private string PluginName { get; } = "Currently Playing Track Info Plugin";
        private IAimpServicePlayer _playerService;
        private bool IsWaitMode;
        private string _path = "./config.json";
        private Timer _updateTimer;
        private WebSocket _webSocketClient;
        private PluginSettings _pluginSettings;
        private FileLogger? _logger;
        private TrackInfoMessage _prevTrack;

        public override void Initialize()
        {
            _playerService = Player.ServicePlayer;
            _pluginSettings = LoadSettings();
            var origin = $"ws://{_pluginSettings.Host}:{_pluginSettings.Port}";
            if (_pluginSettings.DebugMode) _logger = new FileLogger();

            _webSocketClient = new WebSocket($"{origin}/aimp")
            {
                Origin = origin
            };

            _updateTimer = new Timer(_pluginSettings.Interval * 1000) { AutoReset = true };
            _updateTimer.Elapsed += OnTimerElapsed;
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                OnTimerElapsed(null, null);
            });
            _updateTimer.Start();
            _logger?.Write($"{PluginName} Initialized!");
        }

        public override void Dispose()
        {
            _updateTimer.Close();

            _webSocketClient.SendAsync(JsonSerializer.Serialize(new TrackInfoMessage { IsPlaying = false }), null);
            _webSocketClient.Close();
            if (_pluginSettings.DebugMode) _logger?.Close();
        }

        private async void OnTimerElapsed(object? sender, ElapsedEventArgs args)
        {
            _logger?.Write("Timer elapsed");
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

            if (trackInfoMessage.Equals(this._prevTrack))
            {
                _logger?.Write("Same track, skip");
                return Task.CompletedTask;
            }

            this._prevTrack = trackInfoMessage;
            _webSocketClient.Connect();
            _webSocketClient.SendAsync(JsonSerializer.Serialize(trackInfoMessage), null);
            _logger?.Write(nameof(SendTrackInfoAndIsPlaying), trackInfoMessage);
            return Task.CompletedTask;
        }

        private Task WaitModeTask()
        {
            _logger?.Write("Waiting for connectinon.");
            return Task.CompletedTask;
        }

        private Task EnableWaitMode()
        {
            IsWaitMode = true;
            _updateTimer.Interval = _pluginSettings.WaitInterval * 1000;
            _logger?.Write("Wait mode enabled.");
            return Task.CompletedTask;
        }


        private Task DisableWaitMode()
        {
            IsWaitMode = false;
            _updateTimer.Interval = _pluginSettings.Interval * 1000;
            _logger?.Write("Wait mode disabled.");
            return Task.CompletedTask;
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
                    _logger?.Write($"Error while loading settings from your json: {ex.Message}");
                }
            }

            return new PluginSettings();
        }
    }
}