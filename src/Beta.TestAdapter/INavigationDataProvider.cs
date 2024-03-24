namespace Beta.TestAdapter;

/// <summary>
///     Defines a navigation provider for resolving source code locations.
/// </summary>
public interface INavigationDataProvider : IDisposable
{
    /// <summary>
    ///     Gets the navigation data for a test method.
    /// </summary>
    /// <param name="className">The fully qualified name of the class that defined the test.</param>
    /// <param name="methodName">The name of the test method.</param>
    /// <returns>The navigation data for the test method, or null if not found.</returns>
    NavigationData? Get(string className, string methodName);
}
