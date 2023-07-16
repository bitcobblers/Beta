namespace Beta;

public class StepBuilder
{
    public Delegate Handler { get; } = () => { };
    protected BaseTestBuilder Builder { get; }

    public StepBuilder(BaseTestBuilder builder)
    {
        Builder = builder;
    }

    protected StepBuilder(BaseTestBuilder builder, Action handler) : this(builder) => Handler = handler;

    protected StepBuilder(BaseTestBuilder builder, Func<object> handler) : this(builder) => Handler = handler;

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
    protected StepBuilder(BaseTestBuilder builder, Func<TInput>? handler) :
        base(builder, () => handler())
    {
    }

    protected StepBuilder(BaseTestBuilder builder, Action? handler)
        : base(builder, handler)
    {
    }

    protected Func<TOutput> Compile<TOutput>(Func<TInput, TOutput> handler)
    {
        return () =>
        {
            dynamic result = Handler.DynamicInvoke()!;
            return handler(result);
        };
    }

    protected Action CompileAction(Action<TInput> handler)
    {
        return () =>
        {
            var result = Handler.DynamicInvoke();
            handler((TInput)result!);
        };
    }
}
