using System.Diagnostics;

namespace Beta;

public abstract class CoreLogger(string scope, Stopwatch stopwatch) : ICoreLogger
{
    public string Scope => scope;
    protected Stopwatch Stopwatch => stopwatch;

    public abstract void Log(LogLevel level, string message);

    public abstract ICoreLogger CreateScope(string scope);

    protected string FormatMessage(string message)
    {
        var elapsed = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");

        return scope == "/" ?
            $"[Beta {elapsed}] {message}" :
            $"[Beta {elapsed}] {scope}: {message}";
    }
}
