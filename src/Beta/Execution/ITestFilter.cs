namespace Beta.Execution;

/// <summary>
///     Defines a filter for excluding tests from execution.
/// </summary>
public interface ITestFilter
{
    /// <summary>
    ///     Checks if the test should be included in the execution.
    /// </summary>
    /// <param name="test">The test to check.</param>
    /// <returns>True if the test should be included in the execution.</returns>
    bool Filter(BetaTest test);
}