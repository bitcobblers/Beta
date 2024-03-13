// ReSharper disable once CheckNamespace

namespace Beta.Engine.Internal;

/// <summary>
///     Provides internal logging to the NUnit framework
/// </summary>
public class Logger : ILogger
{
    private const string TimeFmt = "HH:mm:ss.fff";
    private const string TraceFmt = "{0} {1,-5} [{2,2}] {3}: {4}";

    private readonly string _name;
    private readonly TextWriter _writer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Logger" /> class.
    /// </summary>
    /// <param name="fullName">The name.</param>
    /// <param name="level">The log level.</param>
    /// <param name="writer">The writer where logs are sent.</param>
    public Logger(string fullName, InternalTraceLevel level, TextWriter writer)
    {
        TraceLevel = level;
        _writer = writer;

        var index = fullName.LastIndexOf('.');
        _name = index >= 0 ? fullName.Substring(index + 1) : fullName;
    }

    public InternalTraceLevel TraceLevel { get; }

    /// <inheritdoc />
    public void Error(string message)
    {
        Log(InternalTraceLevel.Error, message);
    }

    /// <inheritdoc />
    public void Error(string format, params object[] args)
    {
        Log(InternalTraceLevel.Error, format, args);
    }

    /// <inheritdoc />
    public void Warning(string message)
    {
        Log(InternalTraceLevel.Warning, message);
    }

    /// <inheritdoc />
    public void Warning(string format, params object[] args)
    {
        Log(InternalTraceLevel.Warning, format, args);
    }

    /// <inheritdoc />
    public void Info(string message)
    {
        Log(InternalTraceLevel.Info, message);
    }

    /// <inheritdoc />
    public void Info(string format, params object[] args)
    {
        Log(InternalTraceLevel.Info, format, args);
    }

    /// <inheritdoc />
    public void Debug(string message)
    {
        Log(InternalTraceLevel.Verbose, message);
    }

    /// <inheritdoc />
    public void Debug(string format, params object[] args)
    {
        Log(InternalTraceLevel.Verbose, format, args);
    }

    private void Log(InternalTraceLevel level, string message)
    {
        if (TraceLevel >= level)
        {
            WriteLog(level, message);
        }
    }

    private void Log(InternalTraceLevel level, string format, params object[] args)
    {
        if (TraceLevel >= level)
        {
            WriteLog(level, string.Format(format, args));
        }
    }

    private void WriteLog(InternalTraceLevel level, string message)
    {
        _writer.WriteLine(TraceFmt,
            DateTime.Now.ToString(TimeFmt),
            level,
            Environment.CurrentManagedThreadId,
            _name, message);
    }
}
