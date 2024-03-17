namespace Beta.Execution;

/// <summary>
///     Defines a runner used to execute tests.
/// </summary>
public interface ITestRunner
{
    /// <summary>
    ///     Runs the specified tests.
    /// </summary>
    /// <param name="tests">A collection of tests to execute.</param>
    /// <param name="filter">The filter used to exclude tests.</param>
    /// <param name="cancellationToken">The cancellation token use to use for async support.</param>
    /// <returns>An awaitable task for the execution.</returns>
    Task Run(IEnumerable<BetaTest> tests, ITestFilter filter, CancellationToken cancellationToken);
}
