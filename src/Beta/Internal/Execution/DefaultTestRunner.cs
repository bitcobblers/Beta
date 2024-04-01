using Beta.Sdk.Abstractions;
using Beta.Sdk.Interfaces;

namespace Beta.Internal.Execution;

/// <summary>
///     Defines the default test runner.
/// </summary>
public class DefaultTestRunner : ITestRunner
{
    /// <inheritdoc />
    public Task Run(
        IEnumerable<Test> tests,
        ITestFilter filter,
        CancellationToken cancellationToken) => Task.CompletedTask;
}
