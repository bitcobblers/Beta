namespace Beta.Sdk.Abstractions;

/// <summary>
///     Defines the results from running a single test.
/// </summary>
/// <param name="Id">The id of the test.</param>
public record BetaTestResult(Guid Id)
{
    /// <summary>
    ///     Gets the outcome of the test.
    /// </summary>
    public TestOutcome Outcome { get; init; }

    /// <summary>
    ///     Gets the proof results associated with the test.
    /// </summary>
    public List<ProofResult> Results { get; } = [];

    /// <summary>
    ///     Gets the text output for the test.
    /// </summary>
    public string Output { get; init; } = string.Empty;
}
