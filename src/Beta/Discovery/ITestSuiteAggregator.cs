namespace Beta.Discovery;

public interface ITestSuiteAggregator
{
    /// <summary>
    ///     Aggregates all of the suites from the given types.
    /// </summary>
    /// <param name="types">The types to scan for test suites</param>
    /// <returns>A collection of discovered tests</returns>
    IEnumerable<BetaTest> Aggregate(IEnumerable<Type> types);
}
