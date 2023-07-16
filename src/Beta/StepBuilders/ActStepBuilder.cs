using JetBrains.Annotations;

namespace Beta.StepBuilders;

public class ActStepBuilder<TInput> : StepBuilder<GuidedTestBuilder, TInput>
{
    public ActStepBuilder(GuidedTestBuilder builder, Func<TInput> handler)
        : base(builder, handler)
    {
        builder.IsActDefined = true;
    }

    [PublicAPI]
    public AssertStepBuilder<TInput> Assert(Action<TInput> handler) => new(Builder, CompileAction(handler));
}
