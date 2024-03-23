using System.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter;

/// <summary>
///     Defines the default logger that forwards to the vstest logger.
/// </summary>
/// <param name="scope">The scope of the logger.</param>
/// <param name="stopwatch">The stopwatch to track performance metrics.</param>
/// <param name="logger">The underlying logger to write to.</param>
/// <param name="verbosity">The verbosity of messages to write.</param>
public class TestLogger(string scope, Stopwatch stopwatch, IMessageLogger? logger, int verbosity)
    : ITestLogger, IMessageLogger
{
    public TestLogger(IMessageLogger? logger, LogLevel verbosity = LogLevel.Debug)
        : this("/", Stopwatch.StartNew(), logger, (int)verbosity)
    {
    }

    /// <summary>
    ///     Gets a null logger that swallows all messages.
    /// </summary>
    public static ITestLogger Null { get; } = new TestLogger(null, LogLevel.Error);

    /// <inheritdoc />
    public void SendMessage(TestMessageLevel testMessageLevel, string message) =>
        logger?.SendMessage(testMessageLevel, message);

    /// <inheritdoc />
    public string Scope => scope;

    /// <inheritdoc />
    public ITestLogger CreateScope(string newScope) =>
        new TestLogger(FormatScope(newScope), stopwatch, logger, verbosity);

    /// <inheritdoc />
    public void Log(LogLevel level, string message, Exception? ex = null)
    {
        if (!ShouldLog(level))
        {
            return;
        }

        var messageLevel = ToTestMessageLevel(level);

        SendMessage(messageLevel, FormatMessage(message));

        if (ex != null)
        {
            SendMessage(messageLevel, FormatMessage(ex.ToString()));
        }
    }

    internal static TestMessageLevel ToTestMessageLevel(LogLevel level) =>
        level switch
        {
            LogLevel.Debug => TestMessageLevel.Informational,
            LogLevel.Info => TestMessageLevel.Informational,
            LogLevel.Warn => TestMessageLevel.Warning,
            LogLevel.Error => TestMessageLevel.Error,
            _ => TestMessageLevel.Informational
        };

    private string FormatMessage(string message)
    {
        var elapsed = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");

        return scope == "/" ? $"[Beta {elapsed}] {message}" : $"[Beta {elapsed}] {scope}: {message}";
    }

    private string FormatScope(string newScope) =>
        scope == "/" ? $"/{newScope}" : $"{scope}/{newScope}";

    internal bool ShouldLog(LogLevel level) =>
        (int)level >= verbosity;
}
