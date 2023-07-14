using JetBrains.Annotations;

namespace Beta.AAA;

public class ArrangeStepBuilder : StepBuilder
{
    public ArrangeStepBuilder(Action handler)
        : base(handler)
    {
    }

    [PublicAPI]
    public ArrangeStepBuilder Arrange(Action handler) => new (Compile(handler));

    [PublicAPI]
    public ArrangeStepBuilder<T> Arrange<T>(Func<T> handler) => new (Compile(handler));

    [PublicAPI]
    public ActStepBuilder Act(Action handler) => new(Compile(handler));

    [PublicAPI] 
    public ActStepBuilder<T> Act<T>(Func<T> handler) => new(Compile(handler));
}

public class ArrangeStepBuilder<TIn> : StepBuilder<TIn>
{
    public ArrangeStepBuilder(Func<TIn> handler)
        : base(handler)
    {
    }

    [PublicAPI]
    public ArrangeStepBuilder<TIn> Arrange(Action<TIn> handler) => new(Compile<TIn>(handler));

    [PublicAPI]
    public ArrangeStepBuilder<TOut> Arrange<TOut>(Func<TIn, TOut> handler) => new(Compile(handler));

    [PublicAPI]
    public ActStepBuilder<TIn> Act(Action<TIn> handler) => new(Compile<TIn>(handler));

    [PublicAPI]
    public ActStepBuilder<TOut> Act<TOut>(Func<TIn, TOut> handler) => new(Compile(handler));
}