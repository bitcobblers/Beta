namespace Beta.Discovery;

public interface ITestSuiteProcessor
{
    /// <summary>
    ///     Applies the pre-processor.
    /// </summary>
    void PreProcess(object instance);

    /// <summary>
    ///     Applies the post-processor.
    /// </summary>
    /// <param name="instance"></param>
    void PostProcess(object instance);
}
