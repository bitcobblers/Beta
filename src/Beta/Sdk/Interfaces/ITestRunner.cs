using Beta.Sdk.Abstractions;

namespace Beta.Sdk.Interfaces;

/// <summary>
///     Defines a runner used to execute tests.
/// </summary>
public interface ITestRunner
{
    /// <summary>
    ///     Runs the specified tests.
    /// </summary>
    /// <param name="test">The test to execute.</param>
    void Run(Test test);
}