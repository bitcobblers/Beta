namespace Beta;

public class TestContainer
{
    public BetaTest Test<T>(Axiom<T> axiom)
    {
        return new BetaTestNoData<T>(axiom);
    }

    public BetaTest Test<TInput, T>(ITestDataSource<TInput> scenarios, Func<TInput, Axiom<T>> apply)
    {
        return new BetaTestWithData<TInput, T>(scenarios, apply);
    }

    public BetaTest Test<TInput, T>(IEnumerable<TInput> scenarios, Func<TInput, Axiom<T>> apply)
    {
        return new BetaTestWithData<TInput, T>(new RawEnumerableTestDataSource<TInput>(scenarios), apply);
    }
}