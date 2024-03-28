using System.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter;

/// <summary>
///     Defines the default logger that forwards to the vstest logger.
/// </summary>
public class TestLogger : ITestLogger, IMessageLogger
{
    private readonly IMessageLogger? _logger;
    private readonly string[] _scopes;
    private readonly Stopwatch _stopwatch;
    private readonly int _verbosity;

    public TestLogger(IMessageLogger? logger, LogLevel verbosity = LogLevel.Debug)
        : this([], Stopwatch.StartNew(), logger, (int)verbosity)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TestLogger" /> class.
    /// </summary>
    /// <param name="scopes">The scopes of the logger.</param>
    /// <param name="stopwatch">The stopwatch to track performance metrics.</param>
    /// <param name="logger">The underlying logger to write to.</param>
    /// <param name="verbosity">The verbosity of messages to write.</param>
    private TestLogger(IEnumerable<string> scopes, Stopwatch stopwatch, IMessageLogger? logger, int verbosity)
    {
        _scopes = scopes.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        _stopwatch = stopwatch;
        _logger = logger;
        _verbosity = verbosity;

        var formattedScope = string.Join('/', _scopes);

        Scope = string.IsNullOrWhiteSpace(formattedScope)
            ? "/"
            : $"/{formattedScope}";
    }

    /// <summary>
    ///     Gets a null logger that swallows all messages.
    /// </summary>
    public static ITestLogger Null { get; } = new TestLogger(null, LogLevel.Error);

    /// <inheritdoc />
    public void SendMessage(TestMessageLevel testMessageLevel, string message) =>
        _logger?.SendMessage(testMessageLevel, message);

    /// <inheritdoc />
    public string Scope { get; }

    /// <inheritdoc />
    public ITestLogger CreateScope(params string[] newScopes) =>
        new TestLogger(_scopes.Concat(newScopes), _stopwatch, _logger, _verbosity);

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

    private string FormatMessage(string message)
    {
        var elapsed = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");
        return $"[Beta {elapsed}] {Scope}: {message}";
    }

    internal bool ShouldLog(LogLevel level) =>
        (int)level >= _verbosity;

    internal static TestMessageLevel ToTestMessageLevel(LogLevel level) =>
        level switch
        {
            LogLevel.Debug => TestMessageLevel.Informational,
            LogLevel.Info => TestMessageLevel.Informational,
            LogLevel.Warn => TestMessageLevel.Warning,
            LogLevel.Error => TestMessageLevel.Error,
            _ => TestMessageLevel.Informational
        };
}
