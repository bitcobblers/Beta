namespace Beta;

public abstract class BetaTest
{
    public abstract void Prove();
}

public class BetaTestNoData<T>(Axiom<T> test) : BetaTest
{
    public override void Prove()
    {
        throw new NotImplementedException();
    }
}

public class BetaTestWithData<TInput, T>(IEnumerable<TInput> scenarios, Func<TInput, Axiom<T>> apply) : BetaTest
{
    public override void Prove()
    {
        throw new NotImplementedException();
    }
}
