namespace Beta;

/// <summary>
///     Defines a single step in the test.
/// </summary>
/// <param name="handler">The handler to execute for the test.</param>
/// <typeparam name="T">The return type of the handler.</typeparam>
public class Step<T>(Func<T> handler)
{
    /// <summary>
    ///     Evaluates the step function and produces a result.
    /// </summary>
    /// <returns>The result of the step.</returns>
    public T Evaluate() => handler();
}