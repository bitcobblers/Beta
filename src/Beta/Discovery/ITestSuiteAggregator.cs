namespace Beta.Discovery;

/// <summary>
///     Defines an aggregator that can turn a collection of types into a flat list of tests.
/// </summary>
public interface ITestSuiteAggregator
{
    /// <summary>
    ///     Aggregates all of the suites from the given types.
    /// </summary>
    /// <param name="types">The types to scan for test suites</param>
    /// <returns>A collection of discovered tests</returns>
    IEnumerable<Test> Aggregate(IEnumerable<Type> types);
}