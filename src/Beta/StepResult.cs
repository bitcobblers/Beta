namespace Beta;

public class StepResult<T>(string name, Func<T> handler, string description = "<untitled>")
{
    public string Name { get; } = name;
    public string Description { get; } = description;

    public T Execute() => handler();
}

// public class AsyncStepResult<T>(string name, Func<Task<T>> handler, string description = "<untitled>")
// {
//     public string Name { get; } = name;
//     public string Description { get; } = description;
//     public Func<Task<T>> Handler => handler;
//
//     public Task<T> Execute() => handler();
// }

// public class StepResult<TInput, T>(string name, Func<TInput, T> handler, string description = "<untitled>")
// {
//     public string Name { get; } = name;
//     public string Description { get; } = description;
//     public Func<TInput, T> Handler => handler;
//
//     public T Execute(TInput input) => handler(input);
// }

// public static class AsyncStepResultChainer
// {
//     public static async Task<TResult> Select<TIn, TResult>(this AsyncStepResult<TIn> input, Func<TIn, Task<TResult>> selector)
//     {
//         return await selector(await input.Execute());
//     }
//
//     public static TResult Select<TIn, TResult>(this AsyncStepResult<TIn> input, Func<TIn, TResult> selector)
//     {
//         return selector(input.Execute().Result);
//     }
//
//     public static async Task<AsyncStepResult<TResult>> SelectMany<TFirst, TSecond, TResult>(
//         this AsyncStepResult<TFirst> input,
//         Func<TFirst, AsyncStepResult<TSecond>> bind,
//         Func<TFirst, TSecond, TResult> project)
//     {
//         return new AsyncStepResult<TResult>(
//             input.Name,
//             async () =>
//             {
//                 // var inputValue = input.Execute();
//                 var inputValue = await input.Execute();
//                 return project(inputValue, await (bind(inputValue).Execute()));
//             },
//             input.Description);
//     }
// }

public class TestResult<T>(T value)
{
    public T Value { get; } = value;
}

public static class StepResultChainer
{
    // public static StepResult<TResult> Select<TIn, TResult>(this StepResult<TIn> input, Func<TIn, TResult> selector)
    // {
    //     return new StepResult<TResult>(
    //         "ignored", () => selector(input.Execute()), "ignored");
    // }

    // public static TestResult<TResult> Select<TIn, TResult>(this StepResult<TIn> input, Func<TIn, TResult> selector)
    // {
    //     return new TestResult<TResult>(selector(input.Execute()));
    // }

    public static TestResult<TResult> Select<TIn, TResult>(this StepResult<TIn> input, Func<TIn, TResult> selector)
    {
        return new TestResult<TResult>(selector(input.Execute()));
    }

    public static TestResult<TResult> Select<TIn, TResult>(this StepResult<TIn> input, Func<TIn, Task<TResult>> selector)
    {
        // return new TestResult<TResult>(selector(input.Execute()));
        return null;
    }


    public static Axiom<TResult> Select<TIn, TResult>(this TestResult<TIn> input, Func<TIn, TResult> selector)
    {
        return new Axiom<TResult>(selector(input.Value));
    }

    // public static AsyncAxiom<TResult> Select<TIn, TResult>(this TestResult<TIn> input, Func<TIn, Task<TResult>> selector)
    // {
    //     return new AsyncAxiom<TResult>(null);
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
}
