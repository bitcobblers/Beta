namespace Beta.Execution;

/// <summary>
///     Defines a processor that can do pre/post processing of a test class.
/// </summary>
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
