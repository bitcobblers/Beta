namespace Beta;

public class BaseTestBuilder
{
    public Delegate Handler { get; private set; }

    public void SetHandler(Func<StepBuilder, StepBuilder?> steps)
    {
        var stepBuilder = new StepBuilder();
        var configuredSteps = steps(stepBuilder);

        if (configuredSteps != null)
        {
            Handler = configuredSteps.Handler;
        }
    }
}