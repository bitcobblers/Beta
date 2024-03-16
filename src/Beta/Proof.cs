namespace Beta;

public abstract class Proof
{
    public delegate ProofResult ProofHandler<in T>(T actual);

    public abstract IAsyncEnumerable<ProofResult> Test();
}

public class Proof<T>(Task<T> actual) : Proof
{
    private readonly List<ProofHandler<T>> _handlers = new();

    public Proof(T given) : this(Task.FromResult(given))
    {
    }

    public override async IAsyncEnumerable<ProofResult> Test()
    {
        var actualValue = await actual;

        foreach (var handler in _handlers)
        {
            yield return handler(actualValue);
        }
    }

    public Proof<T> Assert(ProofHandler<T> assert)
    {
        _handlers.Add(assert);
        return this;
    }
}
