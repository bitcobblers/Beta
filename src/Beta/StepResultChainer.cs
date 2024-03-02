namespace Beta;

public static class StepResultChainer
{
    public static TestResult<TResult> Select<TIn, TResult>(this Step<TIn> input, Func<TIn, TResult> selector)
    {
        return new TestResult<TResult>(selector(input.Evaluate()));
    }

    public static TResult Select<TIn, TResult>(this TestResult<TIn> input, Func<TIn, TResult> selector)
    {
        return selector(input.Value);
    }

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