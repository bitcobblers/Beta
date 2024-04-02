using Beta.TestAdapter.Models;

namespace Beta.TestAdapter;

public interface IEngineController
{
    /// <summary>
    /// Queries the test assembly for any tests.
    /// </summary>
    /// <returns>A collection of discovered tests.</returns>
    IEnumerable<DiscoveredTest> Query();

    /// <summary>
    /// Executes a single test.
    /// </summary>
    /// <param name="test">The test to execute.</param>
    void Run(TestInvocationRequest test);
}
