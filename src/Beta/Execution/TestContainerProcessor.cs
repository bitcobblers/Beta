namespace Beta.Execution;

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
