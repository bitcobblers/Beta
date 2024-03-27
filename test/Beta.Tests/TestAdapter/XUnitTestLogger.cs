using Beta.TestAdapter;
using Xunit.Abstractions;

namespace Beta.Tests.TestAdapter;

/// <summary>
///     Defines an implementation of the <see cref="ITestLogger" /> that feeds into
///     an instance of <see cref="ITestOutputHelper" />.
/// </summary>
/// <param name="output">The output to redirect messages to.</param>
public class XUnitTestLogger(ITestOutputHelper output) : ITestLogger
{
    /// <inheritdoc />
    public string Scope => string.Empty;

    /// <inheritdoc />
    public void Log(LogLevel level, string message, Exception? ex = null) =>
        output.WriteLine("[{0}] {1} {2}", level, message, ex == null ? string.Empty : $"({ex})");

    /// <inheritdoc />
    public ITestLogger CreateScope(string newScope) => this;
}
