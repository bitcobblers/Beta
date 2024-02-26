namespace Beta;

public abstract class Proof
{
    public abstract IEnumerable<ProofResult> Evaluate();

    public static Proof<TInput, T> New<TInput, T>(IEnumerable<TInput> input, Func<TInput, StepResult<T>> apply,
        Axiom<TInput, T> axiom)
    {
        return new Proof<TInput, T>(input, apply, axiom);
    }

    public static Proof<T> New<T>(StepResult<T> apply, Axiom<T> axiom)
    {
        return new Proof<T>(apply, axiom);
    }
}

public class Proof<T>(
    StepResult<T> apply,
    Axiom<T> axiom) : Proof
{
    public override IEnumerable<ProofResult> Evaluate()
    {
        var result = apply.Execute();
        yield return axiom.Test(result);
    }
};

public class Proof<TInput, T>(
    IEnumerable<TInput> getInput,
    Func<TInput, StepResult<T>> apply,
    Axiom<TInput, T> axiom) : Proof
{
    public override IEnumerable<ProofResult> Evaluate()
    {
        return from input in getInput
               let result = apply(input).Execute()
               select axiom.Test(input, result);
    }
};
