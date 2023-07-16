using JetBrains.Annotations;

namespace Beta.StepBuilders;

public class ArrangeStepBuilder<TContext> : StepBuilder<TContext>
{
    public ArrangeStepBuilder(BaseTestBuilder builder, Func<TContext> handler)
        : base(builder, handler)
    {
    }

    [PublicAPI]
    public ActStepBuilder<TResult> Act<TResult>(Func<TContext, TResult> handler) => new(Builder, Compile(handler));
}
