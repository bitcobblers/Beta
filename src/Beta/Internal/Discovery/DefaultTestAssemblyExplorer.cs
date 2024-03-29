using System.Reflection;
using Beta.Sdk.Abstractions;
using Beta.Sdk.Interfaces;

namespace Beta.Internal.Discovery;

/// <summary>
///     Defines the default assembly explorer.
/// </summary>
public class DefaultTestAssemblyExplorer
    : ITestAssemblyExplorer
{
    private readonly ITestSuiteAggregator _aggregator;
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultTestAssemblyExplorer" /> class.
    /// </summary>
    /// <param name="aggregator">The aggregator to use.</param>
    /// <param name="logger">The logger to use.</param>
    public DefaultTestAssemblyExplorer(ITestSuiteAggregator aggregator, ILogger logger)
    {
        _aggregator = aggregator;
        _logger = logger.CreateScope("explorer");
    }

    /// <inheritdoc />
    public IEnumerable<Test> Explore(Assembly assembly)
    {
        _logger.Debug($"Beginning exploration of assembly [{assembly.FullName}].");

        try
        {
            foreach (var test in _aggregator.Aggregate(assembly.GetTypes()))
            {
                yield return test;
            }
        }
        finally
        {
            _logger.Debug($"Exploration of assembly [{assembly.FullName}] completed.");
        }
    }
}
