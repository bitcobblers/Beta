using System.Runtime.Loader;

namespace Beta.TestAdapter;

/// <summary>
///     Defines common factory methods for object instantiation.
/// </summary>
public static class Factories
{
    /// <summary>
    ///     Defines a factory method to create a new <see cref="IEngineAdapter" /> instance.
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly to load.</param>
    public delegate IEngineAdapter EngineAdapterFactory(string assemblyPath);

    /// <summary>
    ///     Defines a factory method to create a new <see cref="AssemblyLoadContext" /> instance.
    /// </summary>
    public delegate AssemblyLoadContext GetLoadContextFactory(string assemblyPath);

    /// <summary>
    ///     Defines a factory method to create a new <see cref="INavigationDataProvider" /> instance.
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly to load.</param>
    public delegate INavigationDataProvider NavigationDataProviderFactory(string assemblyPath);
}
