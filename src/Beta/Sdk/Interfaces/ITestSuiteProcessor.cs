namespace Beta.Sdk.Interfaces;

/// <summary>
///     Defines a processor that can do pre/post processing of a test class.
/// </summary>
public interface ITestSuiteProcessor
{
    /// <summary>
    ///     Applies the processor.
    /// </summary>
    void Process(TestSuite instance);
}
