namespace Beta.Execution;

/// <summary>
///     Defines a listener used to receive test events.
/// </summary>
public interface ITestListener
{
    /// <summary>
    ///     Triggered whenever a test start.
    /// </summary>
    /// <param name="id">The id of the test that started.</param>
    /// <param name="startTime">The start time of the test.</param>
    void OnStart(Guid id, DateTime startTime);

    /// <summary>
    ///     Triggered whenever a test finishes.
    /// </summary>
    /// <param name="id">The id of the test that finished.</param>
    /// <param name="endTime">The end time of the test.</param>
    /// <param name="outcome">The outcome of the test.</param>
    /// <param name="log">A lot of any messages generated by the test.</param>
    void OnFinish(Guid id, DateTime endTime, TestOutcome outcome, string log);

    /// <summary>
    ///     Triggered whenever a test is updated with a new result.
    /// </summary>
    /// <param name="id">The id of the test that updated.</param>
    /// <param name="result">A new proof result that was created.</param>
    void OnUpdate(Guid id, ProofResult result);
}