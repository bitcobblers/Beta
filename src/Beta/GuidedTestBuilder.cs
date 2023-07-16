using Beta.StepBuilders;
using JetBrains.Annotations;

namespace Beta;

public class GuidedTestBuilder : BaseTestBuilder
{
    [PublicAPI]
    public ArrangeStepBuilder<TContext> Arrange<TContext>(Func<TContext> handler) => new(handler);
}