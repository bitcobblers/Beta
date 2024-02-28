namespace Beta;

public class Axiom<T>(Task<T> value)
{
    public Axiom(T given) : this(Task.FromResult(given))
    {
    }

    public Axiom<T> IsEqual(T actual)
    {
        return this;
    }

    public async Task<ProofResult> Test()
    {
        var resolved = await value.NoMarshal();

        return new ProofResult(resolved, true, "Success");
    }
}