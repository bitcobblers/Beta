namespace Beta;

public abstract class BetaTest
{
    public abstract void Prove();
}

public class BetaTestNoData(Func<Axiom<ProofResult>> test) : BetaTest
{
    public override void Prove()
    {
        throw new NotImplementedException();
    }
}

public class BetaTestWithData<TInput>(IEnumerable<TInput> input, Func<Axiom<ProofResult>> test) : BetaTest
{
    public override void Prove()
    {
        throw new NotImplementedException();
    }
}

// public class BetaTest<T> : BetaTest
// {
//     public Func<StepResult<T>> Apply { get; }
//     public Axiom<T> Axiom { get; }
//
//     public override void Prove()
//     {
//         throw new NotImplementedException();
//     }
// }
//
// public class BetaTest<TData, T>(IEnumerable<TData> input, Func<TData, StepResult<T>> apply, Axiom<TData, T> axiom) : BetaTest
// {
//     public IEnumerable<TData> Input { get; } = input;
//     public Func<TData, StepResult<T>> Apply { get; } = apply;
//     public Axiom<TData, T> Axiom { get; } = axiom;
//
//     public override void Prove()
//     {
//         foreach (var x in Input)
//         {
//             var result = Apply(x);
//             var r2 = result.Execute();
//
//             int y = 5;
//         }
//     }
// }

// public class TestDefinition
// {
// }
