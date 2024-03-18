using System.Reflection;

namespace Beta.Discovery;

/// <summary>
///     Defines the default assembly explorer.
/// </summary>
/// <param name="aggregator">The aggregator to use.</param>
public class DefaultTestAssemblyExplorer(ITestSuiteAggregator aggregator)
    : ITestAssemblyExplorer
{
    /// <inheritdoc />
    public IEnumerable<Test> Explore(Assembly assembly) =>
        aggregator.Aggregate(assembly.GetTypes());
}
