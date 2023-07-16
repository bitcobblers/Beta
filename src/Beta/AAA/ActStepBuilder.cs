using JetBrains.Annotations;

namespace Beta.AAA;

public class ActStepBuilder<TInput> : StepBuilder<TInput>
{
    public ActStepBuilder(Func<TInput> handler)
        : base(handler)
    {
    }

    [PublicAPI]
    public AssertStepBuilder<TInput> Assert(Action<TInput> handler) => new(Compile<TInput>(handler));
}
