namespace Beta;

#pragma warning disable CS9113 // Parameter is unread.
public abstract class BetaTest(object instance)
#pragma warning restore CS9113 // Parameter is unread.
{
    public abstract Task Prove();
}

#pragma warning disable CS9113 // Parameter is unread.
public class BetaTest<T>(object instance, Proof<T> test) : BetaTest(instance)
#pragma warning restore CS9113 // Parameter is unread.
{
    public override Task Prove()
    {
        throw new NotImplementedException();
    }
}
