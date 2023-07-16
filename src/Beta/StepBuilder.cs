namespace Beta;

public class StepBuilder
{
    public Delegate Handler { get; } = () => { };

    public StepBuilder()
    {
    }

    protected StepBuilder(Action handler) => Handler = handler;

    protected StepBuilder(Func<object> handler) => Handler = handler;

    protected Action Compile(Action? handler)
    {
        return () =>
        {
            Handler.DynamicInvoke();
            handler?.Invoke();
        };
    }

    protected Func<T> Compile<T>(Func<T> handler)
    {
        return () =>
        {
            Handler.DynamicInvoke();
            return handler();
        };
    }
}

public class StepBuilder<TInput> : StepBuilder
{
    protected StepBuilder(Func<TInput>? handler) :
        base(() => handler())
    {
    }

    protected Func<TOut> Compile<TOut>(Func<TInput, TOut> handler)
    {
        return () =>
        {
            dynamic result = Handler.DynamicInvoke()!;
            return handler(result);
        };
    }

    protected Func<TInput> Compile<TI>(Action<TInput> handler)
    {
        return () =>
        {
            var result = Handler.DynamicInvoke();
            handler((TInput)result!);

            return (TInput)result!;
        };
    }
}
