namespace Beta;

public class BetaTest
{
}

public class BetaTest<T> : BetaTest
{
    public Func<StepResult<T>> Apply { get; }
    public Axiom<T> Axiom { get; }
}

public class BetaTest<TData, T> : BetaTest
{
    public IEnumerable<TData> Input { get; }
    public Func<TData, StepResult<T>> Apply { get; }
    public Axiom<TData, T> Axiom { get; }
}
