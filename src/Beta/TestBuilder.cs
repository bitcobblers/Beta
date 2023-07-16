namespace Beta;

public class BaseTestBuilder
{
    public Delegate Handler { get; private set; } = () => { };

    public void SetHandler(Func<StepBuilder?> steps)
    {
        var configuredSteps = steps();

        if (configuredSteps != null)
        {
            Handler = configuredSteps.Handler;
        }
    }

    public void UpdateHandler(Delegate handler)
    {
        Handler = handler;
    }
}