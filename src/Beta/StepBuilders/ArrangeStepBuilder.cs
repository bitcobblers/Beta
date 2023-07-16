using JetBrains.Annotations;

namespace Beta.StepBuilders;

public class ArrangeStepBuilder<TInput> : StepBuilder<TInput>
{
    public ArrangeStepBuilder(Func<TInput> handler)
        : base(handler)
    {
    }

    [PublicAPI]
    public ActStepBuilder<TOut> Act<TOut>(Func<TInput, TOut> handler) => new(Compile(handler));
}
