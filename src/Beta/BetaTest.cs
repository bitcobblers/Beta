namespace Beta;

public abstract class BetaTest(object instance)
{
    public abstract void Prove();
}

public class BetaTestNoData<T>(object instance, Axiom<T> test) : BetaTest(instance)
{
    public override void Prove()
    {
        throw new NotImplementedException();
    }
}

public class BetaTestWithData<TInput, T>(
    object instance,
    IScenarioSource<TInput> scenarios,
    Func<TInput, Axiom<T>> apply) : BetaTest(instance)
{
    public override void Prove()
    {
        throw new NotImplementedException();
    }
}