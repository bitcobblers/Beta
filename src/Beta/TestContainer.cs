namespace Beta;

public class TestContainer
{
    // public BetaTest Test(Func<Axiom<ProofResult>> apply)
    // {
    //     return null;
    // }

    public BetaTest Test<T>(Axiom<T> axiom)
    {
        return null;
    }

    public BetaTest Test<TInput, T>(IEnumerable<TInput> data, Func<TInput, Axiom<T>> apply)
    {
        return null;
        //return new BetaTestWithData<TInput>(data);
    }

    // public static TestContainer<T> NewTest<T>(Func<StepResult<T>> apply)
    // {
    //     return new TestContainer<T>(apply);
    // }
    //
    // public TestContainerWithInput<TData> NewTest<TData>(IEnumerable<TData> data)
    // {
    //     return new TestContainerWithInput<TData>(data);
    // }
}

// public class TestContainerWithInput<TData>(IEnumerable<TData> data)
// {
//     public IEnumerable<TData> Data { get; } = data;
//
//     public TestContainer<TData, T> Apply<T>(Func<TData, StepResult<T>> apply)
//     {
//         return new TestContainer<TData, T>(Data, apply);
//     }
// }
//
// public class TestContainer<T>(Func<StepResult<T>> apply)
// {
//     public Func<StepResult<T>> Apply { get; } = apply;
//
//     public BetaTest<T> Proof( Action<Axiom<T>> axiom)
//     {
//         return new BetaTest<T>();
//     }
// }
//
// public class TestContainer<TInput, T>(IEnumerable<TInput> data, Func<TInput, StepResult<T>> apply)
// {
//     // public Func<TInput, StepResult<T>> Apply { get; } = apply;
//
//     public BetaTest<TInput, T> Proof( Action<Axiom<TInput,T>> configureAxiom)
//     {
//         var axiom = new Axiom<TInput, T>();
//         configureAxiom(axiom);
//
//         return new BetaTest<TInput, T>(data, apply, axiom);
//     }
// }