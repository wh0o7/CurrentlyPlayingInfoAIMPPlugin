# Плагин CurrentlyPlayingInfoAIMPPlugin

[![en](https://img.shields.io/badge/lang-en-blue.svg)](https://github.com/wh0o7/CurrentlyPlayingInfoAIMPPlugin/blob/main/README.md) [![ru](https://img.shields.io/badge/lang-ru-red.svg)](https://github.com/wh0o7/CurrentlyPlayingInfoAIMPPlugin/blob/main/README.ru-ru.md)

## Обзор

CurrentlyPlayingInfoAIMPPlugin - это плагин для аудио-плеера AIMP, который отправляет информацию о текущем воспроизводимом треке на WebSocket-сервер через указанные интервалы времени. Если соединение с WebSocket не устанавливается, плагин переходит в режим ожидания и попытки переподключения выполняются через заранее определенные интервалы.

## Установка

1. Скачайте последний релиз с [GitHub Releases](https://github.com/wh0o7/CurrentlyPlayingInfoAIMPPlugin/releases).
2. Создайте директорию с именем `currently_playing_track_info` в директории `Plugins` вашего AIMP.
3. Распакуйте содержимое скачанного архива релиза в директорию `currently_playing_track_info`.

## Конфигурация

Создайте JSON-файл конфигурации с именем `config.json` в директории `currently_playing_track_info`. Опции конфигурации следующие:

```json
{
    "Interval": 40000,
    "WaitInterval": 90000,
    "Host": "127.0.0.1",
    "Port": 5543,
    "DebugMode": false
}
```

- `"Interval"`: Интервал (в миллисекундах) между отправкой информации о треке на WebSocket-сервер.
- `"WaitInterval"`: Интервал (в миллисекундах) ожидания перед повторной попыткой подключения к WebSocket-серверу в случае неудачи.
- `"Host"`: Адрес хоста WebSocket-сервера. (тот же, что и в основном приложении)
- `"Port"`: Номер порта WebSocket-сервера.
- `"DebugMode"`: Установите `true`, чтобы включить режим отладки (дополнительный вывод в консоль), или `false`, чтобы отключить его (по умолчанию).

## Использование

1. Запустите аудио-плеер AIMP.
2. Убедитесь, что плагин установлен и сконфигурирован корректно.
3. Плагин автоматически будет отправлять информацию о треке на WebSocket-сервер согласно указанным интервалам.

**Интеграция с TelegramMusicStatus:**

Этот плагин служит как клиентская часть для проекта [TelegramMusicStatus](https://github.com/wh0o7/TelegramMusicStatus). Плагин собирает информацию о текущем воспроизводимом треке и отправляет ее на указанный WebSocket-сервер через регулярные интервалы. Собранная информация используется проектом TelegramMusicStatus для обновления статуса музыки в вашем профиле Telegram. Совместное использование этих двух проектов значительно улучшает опыт прослушивания музыки, уведомляя ваших друзей о ваших текущих музыкальных предпочтениях. Для получения подробной информации о настройке полной системы, обратитесь к репозиторию [TelegramMusicStatus](https://github.com/wh0o7/TelegramMusicStatus).

## Вклад

Вклад приветствуется! Не стесняйтесь создавать запросы на изменения или открывать запросы на включение через [GitHub](https://github.com/wh0o7/CurrentlyPlayingInfoAIMPPlugin).

## Лицензия

Этот проект выпущен под лицензией [MIT License](LICENSE).
