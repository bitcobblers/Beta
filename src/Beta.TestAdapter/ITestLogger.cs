namespace Beta.TestAdapter;

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
    ///     Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Debug(string message);

    /// <summary>
    ///     Logs an info message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Info(string message);

    /// <summary>
    ///     Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Warning(string message);

    /// <summary>
    ///     Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception to log.</param>
    void Warning(string message, Exception? ex);

    /// <summary>
    ///     Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Error(string message);

    /// <summary>
    ///     Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">The exception to log.</param>
    void Error(string message, Exception? ex);

    /// <summary>
    ///     Creates a new logging scope.
    /// </summary>
    /// <param name="newScope">The new scope to create.</param>
    /// <returns>A new logger for the scope.</returns>
    ITestLogger CreateScope(string newScope);
}
