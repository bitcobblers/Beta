using System.Diagnostics;

namespace Beta;

public abstract class CoreLogger(string scope, Stopwatch stopwatch) : ICoreLogger
{
    protected string Scope => scope;
    protected Stopwatch Stopwatch => stopwatch;

    public abstract void Log(LogLevel level, string message);

    public virtual void Log(LogLevel level, string format, params object?[] args)
    {
        var elapsed = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");
        Log(level, $"[Beta {elapsed}]/{scope}: {string.Format(format, args)}");
    }

    public abstract ICoreLogger CreateScope(string scope);
}
