namespace Beta.Internal;

/// <summary>
///     Implements an internal logger that writes back to the adapter.
/// </summary>
/// <param name="log">The logging callback function to invoke.</param>
public class InternalLogger(Action<int, string, Exception?> log) : ILogger
{
    /// <inheritdoc />
    public void Log(int verbosity, string message, Exception? ex = null) =>
        log(verbosity, message, ex);
}
