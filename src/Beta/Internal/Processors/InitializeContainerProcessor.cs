using Beta.Sdk.Interfaces;

namespace Beta.Internal.Processors;

/// <summary>
///     Defines the processor that can initialize a <see cref="TestSuite.DI" /> instance.
/// </summary>
public class InitializeContainerProcessor : ITestSuiteProcessor
{
    /// <inheritdoc />
    public void Process(TestSuite instance) =>
        (instance as TestSuite.DI)?.Initialize();
}
