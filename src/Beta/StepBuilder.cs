namespace Beta;

public class StepBuilder
{
    public Delegate Handler { get; protected set; } = () => { };

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

public class StepBuilder<TIn> : StepBuilder
{
    protected StepBuilder(Func<TIn> handler) :
        base(() => handler())
    {
    }

    protected Func<TOut> Compile<TOut>(Func<TIn, TOut> handler)
    {
        return () =>
        {
            dynamic result = Handler.DynamicInvoke()!;
            return handler(result);
        };
    }

    protected Func<TIn> Compile<TI>(Action<TIn> handler)
    {
        return () =>
        {
            var result = Handler.DynamicInvoke();
            handler((TIn)result!);

            return (TIn)result!;
        };
    }
}
