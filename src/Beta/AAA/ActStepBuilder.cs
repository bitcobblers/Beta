using JetBrains.Annotations;

namespace Beta.AAA;

//public class ActStepBuilder : StepBuilder
//{
//    public ActStepBuilder(Action handler)
//        : base(handler)
//    {
//    }

//    [PublicAPI]
//    public ActStepBuilder Act(Action handler) => new(Compile(handler));

//    [PublicAPI]
//    public ActStepBuilder<T> Act<T>(Func<T> handler) => new(Compile(handler));

//    [PublicAPI]
//    public AssertStepBuilder Assert(Action handler) => new(Compile(handler));

//    [PublicAPI]
//    public AssertStepBuilder<T> Assert<T>(Func<T> handler) => new(Compile(handler));
//}

public class ActStepBuilder<TInput> : StepBuilder<TInput>
{
    public ActStepBuilder(Func<TInput> handler)
        : base(handler)
    {
    }


    [PublicAPI]
    public AssertStepBuilder<TInput> Assert(Action<TInput> handler) => new(Compile<TInput>(handler));
}