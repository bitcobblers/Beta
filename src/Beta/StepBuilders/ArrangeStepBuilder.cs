using JetBrains.Annotations;

namespace Beta.StepBuilders;

public class ArrangeStepBuilder<TContext> : StepBuilder<GuidedTestBuilder, TContext>
{
    public ArrangeStepBuilder(GuidedTestBuilder builder, Func<TContext> handler)
        : base(builder, handler)
    {
        builder.IsArrangeDefined = true;
    }

    [PublicAPI]
    public ActStepBuilder<TResult> Act<TResult>(Func<TContext, TResult> handler) => new(Builder, Compile(handler));
}
