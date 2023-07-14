namespace Beta;

public class TestBuilder
{
    public string Name { get; }
    public Delegate Handler { get; private set; }

    public TestBuilder(string name)
    {
        Name = name;
    }

    public void AddSteps(Func<StepBuilder, StepBuilder?> steps)
    {
        var stepBuilder = new StepBuilder();
        var configuredSteps = steps(stepBuilder);

        if (configuredSteps != null)
        {
            Handler = configuredSteps.Handler;
        }
    }
}
