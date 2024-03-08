using System.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.Runner.TestAdapter;

public class InternalLogger : CoreLogger
{
    private readonly IMessageLogger? _logger;

    public InternalLogger(IMessageLogger? logger, string scope = "/")
        : this(logger, scope, Stopwatch.StartNew())
    {
    }

    private InternalLogger(IMessageLogger? logger, string scope, Stopwatch stopwatch)
        : base(scope, stopwatch) =>
        _logger = logger;

    public override void Log(LogLevel level, string message)
    {
        _logger?.SendMessage(ToTestMessageLevel(level), FormatMessage(message));
    }

    public override ICoreLogger CreateScope(string newScope) =>
        new InternalLogger(_logger, FormatScope(newScope), Stopwatch);

    private string FormatScope(string newScope) =>
        Scope == "/" ? $"/{newScope}" : $"{Scope}/{newScope}";

    internal static TestMessageLevel ToTestMessageLevel(LogLevel level) =>
        level switch
        {
            LogLevel.Info => TestMessageLevel.Informational,
            LogLevel.Warn => TestMessageLevel.Warning,
            LogLevel.Error => TestMessageLevel.Error,
            _ => TestMessageLevel.Informational,
        };
}
