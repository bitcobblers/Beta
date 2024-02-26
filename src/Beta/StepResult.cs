namespace Beta;

public class StepResult<T>(string name, Func<T> handler, string description = "<untitled>")
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public Func<T> Handler => handler;

    public T Execute() => handler();
}

public class StepResult<TInput, T>(string name, Func<TInput, T> handler, string description = "<untitled>")
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public Func<TInput, T> Handler => handler;

    public T Execute(TInput input) => handler(input);
}

public static class StepResultChainer
{
    public static TResult Select<TIn, TResult>(this StepResult<TIn> input, Func<TIn, TResult> selector)
    {
        return selector(input.Execute());
    }

    // public static StepResult<TResult> Bind<TIn, TResult>(this StepResult<TIn> input, Func<TIn, TResult> bind)
    // {
    //     return new StepResult<TResult>(
    //         "<immediate>",
    //         () => bind(input.Execute()));
    // }

    public static StepResult<TResult> SelectMany<TFirst, TSecond, TResult>(
        this StepResult<TFirst> input,
        Func<TFirst, StepResult<TSecond>> bind,
        Func<TFirst, TSecond, TResult> project)
    {
        return new StepResult<TResult>(
            input.Name,
            () =>
            {
                var inputValue = input.Execute();
                return project(inputValue, bind(inputValue).Execute());
            },
            input.Description);
    }

    // public static StepResult<TResult> SelectMany<TFirst, TSecond, TResult>(
    //     this TFirst input,
    //     Func<TFirst, StepResult<TSecond>> bind,
    //     Func<TFirst, TSecond, TResult> project)
    // {
    //     return new StepResult<TResult>(
    //         "<immediate>",
    //         () => project(input, bind(input).Execute()));
    // }
}