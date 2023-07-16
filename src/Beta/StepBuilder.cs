namespace Beta;

public class StepBuilder<TTestBuilder> where TTestBuilder : BaseTestBuilder
{
    protected Delegate Handler { get; } = () => { };
    protected TTestBuilder Builder { get; }

    protected StepBuilder(TTestBuilder builder, Delegate handler)
    {
        Handler = handler;
        Builder = builder;
        Builder.UpdateHandler(Handler);
    }

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

public class StepBuilder<TTestBuilder, TInput> : StepBuilder<TTestBuilder>
    where TTestBuilder : BaseTestBuilder
{
    protected StepBuilder(TTestBuilder builder, Func<TInput>? handler) :
        base(builder, () => handler())
    {
    }

    protected StepBuilder(TTestBuilder builder, Action? handler)
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
