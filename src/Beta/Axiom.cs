namespace Beta;

public class Axiom
{
    // public static Axiom<TInput, T> From<TInput, T>(Func<TInput, StepResult<T>> ignored)
    // {
    //     return new Axiom<TInput, T>();
    // }

    public static Axiom<T> From<T>(StepResult<T> ignored)
    {
        return new Axiom<T>();
    }
}

public class Axiom<T> : Axiom
{
    public Axiom()
    {

    }

    public Axiom(T value)
    {

    }

    public ProofResult Test(T value)
    {
        return new ProofResult(value, true, "Success");
    }

    public Axiom<T> IsEqual(T value)
    {
        return this;
    }
}

public class AsyncAxiom<T> : Axiom
{
    public AsyncAxiom()
    {

    }

    public AsyncAxiom(Task<T> value)
    {

    }

    public ProofResult Test(T value)
    {
        return new ProofResult(value, true, "Success");
    }
}

// public class Axiom<TInput, T> : Axiom
// {
//     public ProofResult Test(TInput input, T value)
//     {
//         return new ProofResult(input, true, "Success");
//     }
// }