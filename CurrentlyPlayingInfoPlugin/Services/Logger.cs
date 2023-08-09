using System.Text.Json;

namespace AIMP.CurrentlyPlayingInfoPlugin.Services;

class FileLogger
{
    private readonly string _fileLogPath;
    private StreamWriter _logFileStream;

    public FileLogger()
    {
        this._fileLogPath = "./currentlyplayinginfoplugin.log";
        this._logFileStream = File.AppendText(_fileLogPath);
        Init();
    }

    public void Write(string operation, object obj)
    {
        Write($"{operation} : {JsonSerializer.Serialize(obj)}");
        
    }

    public void Write(string message)
    {
        this._logFileStream.WriteLineAsync($"[{DateTime.Now:f}] {message}");
        this._logFileStream.Flush();
        
    }

    public void DeleteLogFile()
    {
        File.Delete(_fileLogPath);
        
    }

    public void Write(Exception exception)
    {
        this._logFileStream.WriteLine($"[{DateTime.Now:f}] {exception}");
        this._logFileStream.Flush();
        
    }

    public void Close()
    {
        this._logFileStream.Flush();
        this._logFileStream.Close();
        
    }

    private void Init()
    {
        this.Write("Init plugin");
    }
}