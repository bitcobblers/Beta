using System.Reflection;

namespace Beta.Discovery;

public class DefaultTestAssemblyExplorer(ITestSuiteAggregator aggregator)
    : ITestAssemblyExplorer
{
    /// <inheritdoc />
    public IEnumerable<Test> Explore(Assembly assembly) =>
        aggregator.Aggregate(assembly.GetTypes());
}
