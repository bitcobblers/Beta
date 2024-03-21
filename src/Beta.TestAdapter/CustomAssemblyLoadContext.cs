using System.Reflection;
using System.Runtime.Loader;

namespace Beta.TestAdapter;

/// <summary>
///     Defines a custom loading context for our test assemblies.
/// </summary>
/// <param name="assemblyLoadPath">The path to the test assembly to load.</param>
/// <remarks>
///     This class was ported from the NUnit source code.
/// </remarks>
public class CustomAssemblyLoadContext(string assemblyLoadPath) : AssemblyLoadContext
{
    private readonly string _basePath = Path.GetDirectoryName(assemblyLoadPath)!;
    private readonly AssemblyDependencyResolver _resolver = new(assemblyLoadPath);

    /// <inheritdoc />
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    /// <summary>
    ///     Loads assemblies that are dependencies, and in the same folder as the parent assembly,
    ///     but are not fully specified in parent assembly deps.json file. This happens when the
    ///     dependencies reference in the csproj file has CopyLocal=false, and for example, the
    ///     reference is a projectReference and has the same output directory as the parent.
    ///     LoadFallback should be called via the CustomAssemblyLoadContext.Resolving callback when
    ///     a dependent assembly of that referred to in a previous 'CustomAssemblyLoadContext.Load' call
    ///     could not be loaded by CustomAssemblyLoadContext.Load nor by the default ALC; to which the
    ///     runtime will fallback when CustomAssemblyLoadContext.Load fails (to let the default ALC
    ///     load system assemblies).
    /// </summary>
    /// <param name="name">The name of the assembly to load.</param>
    /// <returns>
    ///     The loaded assembly.
    /// </returns>
    public Assembly? LoadFallback(AssemblyName name)
    {
        var assemblyPath = Path.Combine(_basePath, name.Name + ".dll");

        return File.Exists(assemblyPath)
            ? LoadFromAssemblyPath(assemblyPath)
            : null;
    }
}