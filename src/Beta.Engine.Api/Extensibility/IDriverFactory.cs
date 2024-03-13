using System.Reflection;
using Beta.Engine.Api.Extensibility;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Extensibility;

/// <summary>
///     Interface implemented by a Type that knows how to create a driver for a test assembly.
/// </summary>
[TypeExtensionPoint(Description = "Supplies a driver to run tests that use a specific test framework.")]
public interface IDriverFactory
{
    /// <summary>
    ///     Gets a flag indicating whether a given AssemblyName
    ///     represents a test framework supported by this factory.
    /// </summary>
    /// <param name="reference">An AssemblyName referring to the possible test framework.</param>
    bool IsSupportedTestFramework(AssemblyName reference);

    /// <summary>
    ///     Gets a driver for a given test assembly and a framework
    ///     which the assembly is already known to reference.
    /// </summary>
    /// <param name="reference">An AssemblyName referring to the test framework.</param>
    /// <returns></returns>
    IFrameworkDriver GetDriver(AssemblyName reference);
}
