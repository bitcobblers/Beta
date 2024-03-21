namespace Beta.Execution;

/// <summary>
///     Defines the processor that can initialize a <see cref="TestContainer" /> instance.
/// </summary>
public class TestContainerProcessor : ITestSuiteProcessor
{
    /// <inheritdoc />
    public void PreProcess(object instance) =>
        (instance as TestContainer)?.Initialize();

    /// <inheritdoc />
    public void PostProcess(object instance)
    {
    }
}