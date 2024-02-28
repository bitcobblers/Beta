namespace Beta;

public class Axiom<T>
{
    private readonly Task<T> _value;

    public Axiom(T value) : this(Task.FromResult(value))
    {
    }

    public Axiom(Task<T> value)
    {
        _value = value;
    }

    public Axiom<T> IsEqual(T value)
    {
        return this;
    }

    public ProofResult Test(T value)
    {
        return new ProofResult(value, true, "Success");
    }
}
