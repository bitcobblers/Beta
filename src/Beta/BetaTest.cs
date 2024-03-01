namespace Beta;

public abstract class BetaTest(object instance)
{
    public abstract Task Prove();
}

public class BetaTest<T>(object instance, Axiom<T> test) : BetaTest(instance)
{
    public override Task Prove()
    {
        throw new NotImplementedException();
    }
}
