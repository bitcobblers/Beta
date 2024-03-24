using System.Diagnostics.CodeAnalysis;
using Beta.Sdk.Interfaces;

namespace Beta.Internal.Execution;

/// <summary>
///     Defines the processor that can initialize a <see cref="TestContainer" /> instance.
/// </summary>
public class TestContainerProcessor : ITestSuiteProcessor
{
    /// <inheritdoc />
    public void PreProcess(object instance) =>
        (instance as TestSuite.DI)?.Initialize();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public void PostProcess(object instance)
    {
    }
}
