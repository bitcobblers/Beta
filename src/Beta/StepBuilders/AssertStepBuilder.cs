namespace Beta.StepBuilders;

public class AssertStepBuilder<TInput> : StepBuilder<GuidedTestBuilder, TInput>
{
    public AssertStepBuilder(GuidedTestBuilder builder, Action handler)
        : base(builder, handler)
    {
    }
}
