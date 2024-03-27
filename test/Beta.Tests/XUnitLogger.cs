using Xunit.Abstractions;

namespace Beta.Tests;

/// <summary>
///     Defines an implementation of the <see cref="ILogger" /> that feeds into
///     an instance of <see cref="ITestOutputHelper" />.
/// </summary>
/// <param name="output">The output to redirect messages to.</param>
public class XUnitLogger(ITestOutputHelper output) : ILogger
{
    /// <inheritdoc />
    public void Log(int verbosity, string message, Exception? ex = null)
    {
        output.WriteLine(message);

        if (ex == null)
        {
            return;
        }

        output.WriteLine(ex.Message);
        output.WriteLine(ex.StackTrace);
    }
}
