using System.Reflection;

namespace Beta.Discovery;

/// <summary>
///     Defines an interface used to explore an assembly for test suites.
/// </summary>
public interface ITestAssemblyExplorer
{
    /// <summary>
    ///     Explores the assembly for any test suites.
    /// </summary>
    /// <param name="assembly">The assembly to explore.</param>
    /// <returns>A flat list of all the tests found in the assembly.</returns>
    IEnumerable<BetaTest> Explore(Assembly assembly);
}
