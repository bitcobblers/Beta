namespace Beta;

/// <summary>
///     Defines the result of a test step chain.
/// </summary>
/// <param name="value">The computed result of the test.</param>
/// <typeparam name="T">The type of result computed by the test.</typeparam>
/// <remarks>
///     This is a glue type used by the LINQ provider to allow test steps to be chained together.
/// </remarks>
public class TestResult<T>(T value)
{
    public T Value { get; } = value;
}