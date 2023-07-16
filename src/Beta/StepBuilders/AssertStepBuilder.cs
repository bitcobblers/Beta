namespace Beta.StepBuilders;

public class AssertStepBuilder<TInput> : StepBuilder<TInput>
{
    public AssertStepBuilder(BaseTestBuilder builder, Action handler)
        : base(builder, handler)
    {
        builder.UpdateHandler(Handler);
    }
}
