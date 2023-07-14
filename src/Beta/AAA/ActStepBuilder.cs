using JetBrains.Annotations;

namespace Beta.AAA;

public class ActStepBuilder : StepBuilder
{
    public ActStepBuilder(Action handler)
        : base(handler)
    {
    }

    [PublicAPI]
    public ActStepBuilder Act(Action handler) => new(Compile(handler));

    [PublicAPI]
    public ActStepBuilder<T> Act<T>(Func<T> handler) => new(Compile(handler));

    [PublicAPI]
    public AssertStepBuilder Assert(Action handler) => new(Compile(handler));

    [PublicAPI]
    public AssertStepBuilder<T> Assert<T>(Func<T> handler) => new(Compile(handler));
}

public class ActStepBuilder<TIn> : StepBuilder<TIn>
{
    public ActStepBuilder(Func<TIn> handler)
        : base(handler)
    {
    }

    [PublicAPI]
    public ActStepBuilder<TIn> Act(Action<TIn> handler) => new(Compile<TIn>(handler));

    [PublicAPI]
    public ActStepBuilder<TOut> Act<TOut>(Func<TIn, TOut> handler) => new(Compile(handler));

    [PublicAPI]
    public AssertStepBuilder<TIn> Assert(Action<TIn> handler) => new(Compile<TIn>(handler));

    [PublicAPI]
    public AssertStepBuilder<TOut> Assert<TOut>(Func<TIn, TOut> handler) => new(Compile(handler));
}