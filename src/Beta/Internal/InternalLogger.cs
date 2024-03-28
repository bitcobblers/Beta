namespace Beta.Internal;

/// <summary>
///     Implements an internal logger that writes back to the adapter.
/// </summary>
/// <param name="log">The logging callback function to invoke.</param>
/// <param name="scopes">The scopes to apply to the logged message.</param>
public class InternalLogger(Action<int, string[], string, Exception?> log, string[] scopes) : ILogger
{
    public InternalLogger(Action<int, string[], string, Exception?> log)
        : this(log, [])
    {
    }

    /// <inheritdoc />
    public void Log(int verbosity, string message, Exception? ex = null) =>
        log(verbosity, scopes, message, ex);

    /// <inheritdoc />
    public ILogger CreateScope(params string[] newScopes) =>
        new InternalLogger(log, scopes.Concat(newScopes).ToArray());
}
