namespace Beta;

public class TestContainer
{
    public BetaTest Test<T>(Axiom<T> axiom)
    {
        return new BetaTestNoData<T>(axiom);
    }

    public BetaTest Test<TInput, T>(IEnumerable<TInput> data, Func<TInput, Axiom<T>> apply)
    {
        return new BetaTestWithData<TInput, T>(data, apply);
    }
}
