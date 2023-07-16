using JetBrains.Annotations;

namespace Beta.StepBuilders;

public class ArrangeStepBuilder<TContext> : StepBuilder<TContext>
{
    public ArrangeStepBuilder(Func<TContext> handler)
        : base(handler)
    {
    }

    [PublicAPI]
    public ActStepBuilder<TResult> Act<TResult>(Func<TContext, TResult> handler) => new(Compile(handler));
}
