namespace Beta;

public delegate ProofResult ProofHandler<in T>(T actual);

public class Axiom<T>(Task<T> actual)
{
    private readonly List<ProofHandler<T>> _handlers = [];

    public Axiom(T given) : this(Task.FromResult(given))
    {
    }

    public Axiom<T> Assert(ProofHandler<T> assert)
    {
        _handlers.Add(assert);
        return this;
    }

    public async IAsyncEnumerable<ProofResult> Test()
    {
        var actualValue = await actual;

        foreach (var handler in _handlers)
        {
            yield return handler(actualValue);
        }
    }
}
