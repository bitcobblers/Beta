namespace Beta.Discovery;

// ReSharper disable once ParameterTypeCanBeEnumerable.Local
public class DefaultTestSuiteAggregator(ITestDiscoverer[] discoverers) : ITestSuiteAggregator
{
    /// <inheritdoc />
    public IEnumerable<BetaTest> Aggregate(IEnumerable<Type> types) =>
        from type in types
        let discoverer = discoverers.FirstOrDefault(d => d.IsSuite(type))
        where discoverer is not null
        from test in discoverer.Discover(type)
        select test;
}
