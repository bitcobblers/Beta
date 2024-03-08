using System.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.Runner.TestAdapter;

public class InternalLogger : CoreLogger
{
    private readonly IMessageLogger? _logger;

    public InternalLogger(IMessageLogger? logger, string scope = "*")
        : this(logger, scope, Stopwatch.StartNew())
    {
    }

    private InternalLogger(IMessageLogger? logger, string scope, Stopwatch stopwatch)
        : base(scope, stopwatch) =>
        this._logger = logger;


    public override void Log(LogLevel level, string message)
    {
        var testMessageLevel = level switch
        {
            LogLevel.Info => TestMessageLevel.Informational,
            LogLevel.Warn => TestMessageLevel.Warning,
            LogLevel.Error => TestMessageLevel.Error,
            _ => TestMessageLevel.Informational,
        };

        _logger?.SendMessage(testMessageLevel, message);
    }

    public override ICoreLogger CreateScope(string newScope)
    {
        return new InternalLogger(_logger, $"{Scope}/{newScope}", Stopwatch);
    }
}
