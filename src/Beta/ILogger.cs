namespace Beta;

/// <summary>
///     Defines a logger that can be used for internal messages.
/// </summary>
public interface ILogger
{
    /// <summary>
    ///     Logs a message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Log(string message);
}
