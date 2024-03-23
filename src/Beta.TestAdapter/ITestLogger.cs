namespace Beta.TestAdapter;

/// <summary>
///     Defines the various log levels.
/// </summary>
public enum LogLevel
{
    Debug = 0,
    Info = 1,
    Warn = 2,
    Error = 3
}

/// <summary>
///     Defines an internal logger interface for use by the runner.
/// </summary>
public interface ITestLogger
{
    /// <summary>
    ///     Gets the scope of the logger.
    /// </summary>
    string Scope { get; }

    /// <summary>
    ///     Writes a log message.
    /// </summary>
    /// <param name="level">The level of the message to write.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="ex">An optional exception to include.</param>
    void Log(LogLevel level, string message, Exception? ex = null);

    /// <summary>
    ///     Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Debug(string message) =>
        Log(LogLevel.Debug, message);

    /// <summary>
    ///     Logs an info message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Info(string message) =>
        Log(LogLevel.Info, message);

    /// <summary>
    ///     Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Warning(string message) =>
        Log(LogLevel.Warn, message);

    /// <summary>
    ///     Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception to log.</param>
    void Warning(string message, Exception? ex) =>
        Log(LogLevel.Warn, message, ex);

    /// <summary>
    ///     Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Error(string message) =>
        Log(LogLevel.Error, message);

    /// <summary>
    ///     Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception to log.</param>
    void Error(string message, Exception? ex) =>
        Log(LogLevel.Error, message, ex);

    /// <summary>
    ///     Creates a new logging scope.
    /// </summary>
    /// <param name="newScope">The new scope to create.</param>
    /// <returns>A new logger for the scope.</returns>
    ITestLogger CreateScope(string newScope);
}
