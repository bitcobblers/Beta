namespace Beta.StepBuilders;

public class AssertStepBuilder<TIn> : StepBuilder<TIn>
{
    public AssertStepBuilder(Func<TIn> handler)
        : base(handler)
    {
    }
}
