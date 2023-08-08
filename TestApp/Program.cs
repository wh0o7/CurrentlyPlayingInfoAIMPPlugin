using System.Net;
using WebSocketSharp.Server;
using WebSocketSharp;

public interface IWebSocketService
{
    void Connect();
}

public class WebSocketService : IWebSocketService
{
    public static bool IsPlaying { get; private set; }
    public static string? Artist { get; private set; }
    public static string? TrackTitle { get; private set; }

    private class APIService : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var receivedMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<TrackInfoMessage>(e.Data);

            IsPlaying = receivedMessage.IsPlaying;
            Artist = receivedMessage.Artist;
            TrackTitle = receivedMessage.TrackTitle;

            Console.WriteLine($"{IsPlaying}, {Artist}, {TrackTitle}");
        }
    }

    public void Connect()
    {
        var wssv = new WebSocketServer($"ws://127.0.0.1:5543");
        wssv.AddWebSocketService<APIService>("/aimp");
        wssv.Start();
        Console.WriteLine("WebSocket Server started.");
        Console.WriteLine(wssv.Address.ToString());
        Console.WriteLine("port: " + wssv.Port);
    }
}


public class Program
{
    public static async Task Main()
    {
        var ws = new WebSocketService();
        ws.Connect();
        await Task.Delay(-1);
    }
}

class TrackInfoMessage
{
    public string TrackTitle { get; set; }
    public string Artist { get; set; }
    public bool IsPlaying { get; set; }
}