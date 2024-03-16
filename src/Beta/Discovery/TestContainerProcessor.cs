namespace Beta.Discovery;

public class TestContainerProcessor : ITestSuiteProcessor
{
    /// <inheritdoc />
    public void PreProcess(object instance) =>
        (instance as TestContainer)?.Prepare();

    /// <inheritdoc />
    public void PostProcess(object instance)
    {
    }
}
