namespace Beta;

public class StepBuilder
{
    protected readonly Action _handler = () => { };

    public StepBuilder()
    {
    }

    protected StepBuilder(Action handler)
    {
        _handler = handler;
    }

    protected Action Compile(Action? handler)
    {
        return () =>
        {
            _handler?.Invoke();
            handler?.Invoke();
        };
    }

    protected Func<T> Compile<T>(Func<T> handler)
    {
        return () =>
        {
            _handler?.Invoke();
            return handler();
        };
    }
}

public class StepBuilder<TIn> : StepBuilder
{
    protected readonly Func<TIn> _handler;

    protected StepBuilder(Func<TIn> handler) => _handler = handler;

    protected Func<TOut> Compile<TOut>(Func<TIn, TOut> handler)
    {
        return () =>
        {
            var result = _handler();
            return handler(result);
        };
    }

    protected Func<TIn> Compile<TI>(Action<TIn> handler)
    {
        return () =>
        {
            var result = _handler();
            handler(result);
            return result;
        };
    }
}
