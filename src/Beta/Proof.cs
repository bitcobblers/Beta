namespace Beta;

public delegate ProofResult ProofHandler<in T>(T actual);

public class Proof<T>(Task<T> actual)
{
    private readonly List<ProofHandler<T>> _handlers = [];

    public Proof(T given) : this(Task.FromResult(given))
    {
    }

    public Task<T> Actual => actual;

    public Proof<T> Assert(ProofHandler<T> assert)
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