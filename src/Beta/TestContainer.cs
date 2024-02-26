namespace Beta;

public class TestContainer
{
    public static TestContainer<T> NewTest<T>(Func<StepResult<T>> apply)
    {
        return new TestContainer<T>(apply);
    }

    public TestContainerWithInput<TData> NewTest<TData>(IEnumerable<TData> data)
    {
        return new TestContainerWithInput<TData>(data);
    }
}

public class TestContainerWithInput<TData>(IEnumerable<TData> data)
{
    public IEnumerable<TData> Data { get; } = data;

    public TestContainer<TData, T> Apply<T>(Func<TData, StepResult<T>> apply)
    {
        return new TestContainer<TData, T>(apply);
    }
}

public class TestContainer<T>(Func<StepResult<T>> apply)
{
    public Func<StepResult<T>> Apply { get; } = apply;

    public BetaTest<T> Proof( Action<Axiom<T>> axiom)
    {
        return new BetaTest<T>();
    }
}

public class TestContainer<TInput, T>(Func<TInput, StepResult<T>> apply)
{
    public Func<TInput, StepResult<T>> Apply { get; } = apply;
    public BetaTest<TInput, T> Proof( Action<Axiom<TInput,T>> axiom)
    {
        return new BetaTest<TInput, T>();
    }
}