namespace Beta;

public class TestBuilder
{
    public TestBuilder(string? name)
    {
    }

    public void AddSteps(Func<StepBuilder, StepBuilder> steps)
    {
        var stepBuilder = new StepBuilder();
        var configuredSteps = steps(stepBuilder);
    }
}
