using Beta.AAA;

namespace Beta;

public class BaseTestBuilder
{
    public Delegate Handler { get; private set; }

    public void AddSteps(Func<StepBuilder, StepBuilder?> steps)
    {
        var stepBuilder = new StepBuilder();
        var configuredSteps = steps(stepBuilder);

        if (configuredSteps != null)
        {
            Handler = configuredSteps.Handler;
        }
    }

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

public class BasicTestBuilder : BaseTestBuilder
{

}

public class GuidedTestBuilder : BaseTestBuilder
{
    public ArrangeStepBuilder Arrange(Action handler) => new(handler);
}
