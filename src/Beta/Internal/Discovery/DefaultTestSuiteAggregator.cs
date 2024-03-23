using Beta.Sdk.Abstractions;
using Beta.Sdk.Interfaces;

namespace Beta.Internal.Discovery;

/// <summary>
///     Defines the default test suite aggregator.
/// </summary>
/// <param name="discoverers"></param>
// ReSharper disable once ParameterTypeCanBeEnumerable.Local
public class DefaultTestSuiteAggregator(IEnumerable<ITestDiscoverer> discoverers) : ITestSuiteAggregator
{
    /// <inheritdoc />
    public IEnumerable<Test> Aggregate(IEnumerable<Type> types)
    {
        return from type in types
               let discoverer = discoverers.FirstOrDefault(d => d.IsSuite(type))
               where discoverer is not null
               from test in discoverer.Discover(type)
               select test;
    }
}
