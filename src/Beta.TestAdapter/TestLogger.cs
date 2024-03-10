using System.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter;

public enum LogLevel
{
    Debug = 0,
    Info = 1,
    Warn = 2,
    Error = 3
}

public interface ITestLogger
{
    int Verbosity { get; }
    string Scope { get; }

    void Debug(string message);
    void Info(string message);
    void Warning(string message);
    void Warning(string message, Exception? ex);
    void Error(string message);
    void Error(string message, Exception? ex);

    ITestLogger CreateScope(string newScope);
}

public class TestLogger(string scope, Stopwatch stopwatch, IMessageLogger? logger) : IMessageLogger, ITestLogger
{
    public TestLogger()
        : this(null, LogLevel.Error)
    {
    }

    public TestLogger(IMessageLogger? logger, LogLevel verbosity)
        : this("/", Stopwatch.StartNew(), logger)
    {
        Verbosity = (int)verbosity;
    }

    /// <inheritdoc />
    public void SendMessage(TestMessageLevel testMessageLevel, string message)
    {
        logger?.SendMessage(testMessageLevel, FormatMessage(message));
    }

    /// <inheritdoc />
    public string Scope => scope;

    /// <inheritdoc />
    public int Verbosity { get; }

    /// <inheritdoc />
    public void Debug(string message)
    {
        Log(LogLevel.Debug, message);
    }

    /// <inheritdoc />
    public void Info(string message)
    {
        Log(LogLevel.Info, message);
    }

    /// <inheritdoc />
    public void Warning(string message)
    {
        Warning(message, null);
    }

    /// <inheritdoc />
    public void Warning(string message, Exception? ex)
    {
        Log(LogLevel.Warn, message);

        if (ex != null)
        {
            Log(LogLevel.Warn, ex.ToString());
        }
    }

    /// <inheritdoc />
    public void Error(string message)
    {
        Error(message, null);
    }

    /// <inheritdoc />
    public void Error(string message, Exception? ex)
    {
        Log(LogLevel.Error, message);

        if (ex != null)
        {
            Log(LogLevel.Error, ex.ToString());
        }
    }

    /// <inheritdoc />
    public ITestLogger CreateScope(string newScope)
    {
        return new TestLogger(FormatScope(newScope), stopwatch, logger);
    }

    private void Log(LogLevel level, string message)
    {
        if (ShouldLog(level))
        {
            SendMessage(ToTestMessageLevel(level), message);
        }
    }

    internal string FormatMessage(string message)
    {
        var elapsed = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");

        return scope == "/" ? $"[Beta {elapsed}] {message}" : $"[Beta {elapsed}] {scope}: {message}";
    }

    internal string FormatScope(string newScope)
    {
        return scope == "/" ? $"/{newScope}" : $"{scope}/{newScope}";
    }

    internal static TestMessageLevel ToTestMessageLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => TestMessageLevel.Informational,
            LogLevel.Info => TestMessageLevel.Informational,
            LogLevel.Warn => TestMessageLevel.Warning,
            LogLevel.Error => TestMessageLevel.Error,
            var _ => TestMessageLevel.Informational
        };
    }

    internal bool ShouldLog(LogLevel level)
    {
        return (int)level >= Verbosity;
    }
}
