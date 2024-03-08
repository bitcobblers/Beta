namespace Beta;

public enum LogLevel
{
    Info,
    Warn,
    Error,
}

public interface ICoreLogger
{
    string Scope { get; }

    void Log(LogLevel level, string message);

    ICoreLogger CreateScope(string newScope);
}