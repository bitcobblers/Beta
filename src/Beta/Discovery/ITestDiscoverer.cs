namespace Beta.Discovery;

public interface ITestDiscoverer
{
    /// <summary>
    ///     Checks whether the given type contains tests.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type contains tests.</returns>
    bool IsSuite(Type type);

    /// <summary>
    /// </summary>
    /// <param name="type">The type to perform discovery on.</param>
    /// <returns>A collection of tests found in the type.</returns>
    IEnumerable<Test> Discover(Type type);
}
