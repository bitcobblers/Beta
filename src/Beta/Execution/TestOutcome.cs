namespace Beta.Execution;

/// <summary>
///     Defines the outcome of a single test.
/// </summary>
public enum TestOutcome
{
    /// <summary>
    ///     Passing test.
    /// </summary>
    Passed,

    /// <summary>
    ///     Failed test.
    /// </summary>
    Failed,

    /// <summary>
    ///     Skipped test.
    /// </summary>
    Skipped,

    /// <summary>
    ///     Inconclusive test.  Occurs when there are no assertions.
    /// </summary>
    Inconclusive,

    /// <summary>
    ///     Error test.  Occurs when an exception is thrown during the test.
    /// </summary>
    Error
}
