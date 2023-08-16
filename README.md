# CurrentlyPlayingInfoAIMPPlugin

[![en](https://img.shields.io/badge/lang-en-blue.svg)](https://github.com/wh0o7/CurrentlyPlayingInfoAIMPPlugin/blob/main/README.md) [![ru](https://img.shields.io/badge/lang-ru-red.svg)](https://github.com/wh0o7/CurrentlyPlayingInfoAIMPPlugin/blob/main/README.ru-ru.md)

## Overview

CurrentlyPlayingInfoAIMPPlugin is a plugin for the AIMP audio player that sends information about the currently playing track to a WebSocket server at specified intervals. If the WebSocket connection fails, the plugin enters a waiting mode and attempts to reconnect at predefined intervals.

## Installation

1. Download the latest release from [GitHub Releases](https://github.com/wh0o7/CurrentlyPlayingInfoAIMPPlugin/releases).
2. Create a directory named `currently_playing_track_info` inside your AIMP's `Plugins` directory.
3. Extract the contents of the downloaded release archive into the `currently_playing_track_info` directory.

## Configuration

Create a JSON configuration file named `config.json` in the `currently_playing_track_info` directory. The configuration options are as follows:

```json
{
    "Interval": 40,
    "WaitInterval": 90,
    "Host": "127.0.0.1",
    "Port": 5543,
    "DebugMode": false
}
```

- `"Interval"`: The interval (in seconds) between sending track information to the WebSocket server. Min 10. Max 300.
- `"WaitInterval"`: The interval (in seconds) to wait before attempting to reconnect to the WebSocket server in case of failure. Min 20. Max 600.
- `"Host"`: The host address of the WebSocket server.(same as in main app
- `"Port"`: The port number of the WebSocket server.
- `"DebugMode"`: Set to `true` to enable debug mode (additional console output), or `false` to disable it(default).

## Usage

1. Start the AIMP audio player.
2. Make sure the plugin is installed and configured correctly.
3. The plugin will automatically send track information to the WebSocket server according to the specified intervals.

**Integration with TelegramMusicStatus:**

This plugin serves as the client-side component for the [TelegramMusicStatus](https://github.com/wh0o7/TelegramMusicStatus) project. The plugin gathers information about the currently playing track and sends it to the specified WebSocket server at regular intervals. This collected information is then utilized by the TelegramMusicStatus project to update the music status on your Telegram profile. By seamlessly working together, these two projects enhance your music listening experience by keeping your friends informed about your current music choices. For more details on setting up the complete system, please refer to the [TelegramMusicStatus](https://github.com/wh0o7/TelegramMusicStatus) repository.

## Contributing

Contributions are welcome! Feel free to open issues or pull requests on [GitHub](https://github.com/wh0o7/CurrentlyPlayingInfoAIMPPlugin/issues).

## License

This project is licensed under the [MIT License](LICENSE).
