using System.Diagnostics.CodeAnalysis;

namespace Beta;

/// <summary>
///     Defines a logger that can be used for internal messages.
/// </summary>
[PublicAPI]
public interface ILogger
{
    /// <summary>
    ///     Writes a debug message.
    /// </summary>
    /// <param name="message">The debug message to write.</param>
    [ExcludeFromCodeCoverage]
    void Debug(string message) =>
        Log(Level.Debug, message);

    /// <summary>
    ///     Write an info message.
    /// </summary>
    /// <param name="message">The info message to write.</param>
    [ExcludeFromCodeCoverage]
    void Info(string message) =>
        Log(Level.Info, message);

    /// <summary>
    ///     Writes a warning message.
    /// </summary>
    /// <param name="message">The warning message to write.</param>
    [ExcludeFromCodeCoverage]
    void Warn(string message) =>
        Warn(message, null);

    /// <summary>
    ///     Writes a warning message with exception.
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <param name="ex">An optional exception to write.</param>
    [ExcludeFromCodeCoverage]
    void Warn(string message, Exception? ex) =>
        Log(Level.Warn, message, ex);

    /// <summary>
    ///     Writes an error message.
    /// </summary>
    /// <param name="message">The error message to write.</param>
    [ExcludeFromCodeCoverage]
    void Error(string message) =>
        Error(message, null);

    /// <summary>
    ///     Writes an error message with exception.
    /// </summary>
    /// <param name="message">The error message to write.</param>
    /// <param name="ex">The optional exception to write.</param>
    [ExcludeFromCodeCoverage]
    void Error(string message, Exception? ex) =>
        Log(Level.Error, message, ex);

    /// <summary>
    ///     Logs a message.
    /// </summary>
    /// <param name="verbosity">The verbosity level of the message.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="ex">An optional exception to log.</param>
    void Log(int verbosity, string message, Exception? ex = null);

    /// <summary>
    ///     Defines the verbosity level of a message.
    /// </summary>
    private static class Level
    {
        public const int Debug = 0;
        public const int Info = 1;
        public const int Warn = 2;
        public const int Error = 3;
    }
}
