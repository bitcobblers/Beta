namespace Beta.Discovery;

/// <summary>
///     Defines the default test suite aggregator.
/// </summary>
/// <param name="discoverers"></param>
// ReSharper disable once ParameterTypeCanBeEnumerable.Local
public class DefaultTestSuiteAggregator(ITestDiscoverer[] discoverers) : ITestSuiteAggregator
{
    /// <inheritdoc />
    public IEnumerable<Test> Aggregate(IEnumerable<Type> types) =>
        from type in types
        let discoverer = discoverers.FirstOrDefault(d => d.IsSuite(type))
        where discoverer is not null
        from test in discoverer.Discover(type)
        select test;
}
