using Beta.Sdk.Abstractions;
using Beta.Sdk.Interfaces;

namespace Beta.Internal.Execution;

/// <summary>
///     Defines the default test runner.
/// </summary>
/// <param name="logger">The internal logger to use.</param>
public class DefaultTestRunner(ILogger logger) : ITestRunner
{
    /// <inheritdoc />
    public Task Run(
        IEnumerable<Test> tests,
        ITestFilter filter,
        CancellationToken cancellationToken) => Task.CompletedTask;
}
