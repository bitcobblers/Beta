namespace Beta;

public enum LogLevel
{
    Info,
    Warn,
    Error,
}

public interface ICoreLogger
{
    void Log(LogLevel level, string message);
    void Log(LogLevel level, string format, params object?[] args);

    ICoreLogger CreateScope(string newScope);
}