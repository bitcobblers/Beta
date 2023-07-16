namespace Beta.StepBuilders;

public class AssertStepBuilder<TInput> : StepBuilder<TInput>
{
    public AssertStepBuilder(Action handler)
        : base(handler)
    {
    }
}
