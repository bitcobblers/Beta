using System.Reflection;

namespace Beta.Discovery;

public class DefaultTestAssemblyExplorer(ITestSuiteAggregator aggregator)
    : ITestAssemblyExplorer
{
    /// <inheritdoc />
    public IEnumerable<BetaTest> Explore(Assembly assembly) =>
        aggregator.Aggregate(assembly.GetTypes());
}
