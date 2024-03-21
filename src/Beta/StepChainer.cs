namespace Beta;

/// <summary>
///     Implements a LINQ provider to allow test steps to be chained together.
/// </summary>
public static class StepChainer
{
    /// <summary>
    ///     Projects the last step into a test result.
    /// </summary>
    /// <param name="input">The last step in the chain.</param>
    /// <param name="selector">The selector to create the test result from.</param>
    /// <typeparam name="TIn">The input type of the steps.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <returns>The projected <see cref="TestResult{T}" />.</returns>
    public static TestResult<TResult> Select<TIn, TResult>(this Step<TIn> input, Func<TIn, TResult> selector) =>
        new(selector(input.Evaluate()));

    /// <summary>
    ///     Used to Project the <see cref="TestResult{T}" /> of the test into a new proof.
    /// </summary>
    /// <param name="input">The test result.</param>
    /// <param name="selector">The selector to create a proof from.</param>
    /// <typeparam name="TIn">The input type of the result.</typeparam>
    /// <typeparam name="TResult">The projected <see cref="Proof{T}" />.</typeparam>
    /// <returns></returns>
    public static TResult Select<TIn, TResult>(this TestResult<TIn> input, Func<TIn, TResult> selector) =>
        selector(input.Value);

    /// <summary>
    ///     Chains two steps together.
    /// </summary>
    /// <param name="input">The input step.</param>
    /// <param name="bind">The binding method.</param>
    /// <param name="project">The projector.</param>
    /// <typeparam name="TFirst">The result type of the first step.</typeparam>
    /// <typeparam name="TSecond">The result type of the second step.</typeparam>
    /// <typeparam name="TResult">The result type of the chained step.</typeparam>
    /// <returns>The next step in the chain.</returns>
    public static Step<TResult> SelectMany<TFirst, TSecond, TResult>(
        this Step<TFirst> input,
        Func<TFirst, Step<TSecond>> bind,
        Func<TFirst, TSecond, TResult> project)
    {
        return new Step<TResult>(
            () =>
            {
                var inputValue = input.Evaluate();
                return project(inputValue, bind(inputValue).Evaluate());
            });
    }
}