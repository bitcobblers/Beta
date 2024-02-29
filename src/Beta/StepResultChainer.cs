namespace Beta;

public static class StepResultChainer
{
    public static TestResult<TResult> Select<TIn, TResult>(this StepResult<TIn> input, Func<TIn, TResult> selector)
    {
        return new TestResult<TResult>(selector(input.Resolve()));
    }

    public static TResult Select<TIn, TResult>(this TestResult<TIn> input, Func<TIn, TResult> selector)
    {
        return selector(input.Value);
    }

    public static StepResult<TResult> SelectMany<TFirst, TSecond, TResult>(
        this StepResult<TFirst> input,
        Func<TFirst, StepResult<TSecond>> bind,
        Func<TFirst, TSecond, TResult> project)
    {
        return new StepResult<TResult>(
            () =>
            {
                var inputValue = input.Resolve();
                return project(inputValue, bind(inputValue).Resolve());
            });
    }
}