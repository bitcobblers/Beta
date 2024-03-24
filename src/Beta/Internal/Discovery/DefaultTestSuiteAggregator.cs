using Beta.Sdk.Abstractions;
using Beta.Sdk.Interfaces;

namespace Beta.Internal.Discovery;

/// <summary>
///     Defines the default test suite aggregator.
/// </summary>
public class DefaultTestSuiteAggregator : ITestSuiteAggregator
{
    private readonly ITestDiscoverer[] _discoverers;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultTestSuiteAggregator" /> class.
    /// </summary>
    /// <param name="discoverers">The collection of discoverers to use.</param>
    public DefaultTestSuiteAggregator(IEnumerable<ITestDiscoverer> discoverers) =>
        _discoverers = discoverers.ToArray();

    /// <inheritdoc />
    public IEnumerable<Test> Aggregate(IEnumerable<Type> types)
    {
        return from type in types
               let discoverer = _discoverers.FirstOrDefault(d => d.IsSuite(type))
               where discoverer is not null
               from test in discoverer.Discover(type)
               select test;
    }
}
