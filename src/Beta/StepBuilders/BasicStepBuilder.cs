namespace Beta.StepBuilders;

public class BasicStepBuilder : StepBuilder
{
    public BasicStepBuilder(BaseTestBuilder builder, Action handler)
        : base(builder, handler)
    {
    }
}
