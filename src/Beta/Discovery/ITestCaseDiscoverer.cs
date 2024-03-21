using System.Reflection;

namespace Beta.Discovery;

/// <summary>
///     Defines a discoverer that can find tests in a given method.
/// </summary>
public interface ITestCaseDiscoverer
{
    /// <summary>
    ///     Discovers test cases within a method.
    /// </summary>
    /// <param name="method">The method to inspect.</param>
    /// <returns>A collection of tests.</returns>
    IEnumerable<Test> Discover(MethodInfo method);
}