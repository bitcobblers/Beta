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

public class TestLogger(string scope, Stopwatch stopwatch, IMessageLogger? logger, int verbosity) :
    ITestLogger, IMessageLogger
{
    public TestLogger(IMessageLogger? logger, LogLevel verbosity = LogLevel.Debug)
        : this("/", Stopwatch.StartNew(), logger, (int)verbosity)
    {
    }

    public static ITestLogger Null { get; } = new TestLogger(null, LogLevel.Error);

    /// <inheritdoc />
    public void SendMessage(TestMessageLevel testMessageLevel, string message)
    {
        logger?.SendMessage(testMessageLevel, message);
    }

    /// <inheritdoc />
    public int Verbosity { get; } = verbosity;

    /// <inheritdoc />
    public string Scope => scope;

    /// <inheritdoc />
    public void Debug(string message) => Log(LogLevel.Debug, message);

    /// <inheritdoc />
    public void Info(string message) => Log(LogLevel.Info, message);

    /// <inheritdoc />
    public void Warning(string message) => Warning(message, null);

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
    public void Error(string message) => Error(message, null);

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
    public ITestLogger CreateScope(string newScope) =>
        new TestLogger(FormatScope(newScope), stopwatch, logger, verbosity);

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

    private void Log(LogLevel level, string message)
    {
        if (ShouldLog(level))
        {
            SendMessage(ToTestMessageLevel(level), FormatMessage(message));
        }
    }

    private string FormatMessage(string message)
    {
        var elapsed = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");

        return scope == "/" ? $"[Beta {elapsed}] {message}" : $"[Beta {elapsed}] {scope}: {message}";
    }

    private string FormatScope(string newScope) => scope == "/" ? $"/{newScope}" : $"{scope}/{newScope}";

    private bool ShouldLog(LogLevel level) => (int)level >= Verbosity;
}
