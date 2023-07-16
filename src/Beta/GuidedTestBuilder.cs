using Beta.StepBuilders;
using JetBrains.Annotations;

namespace Beta;

public class GuidedTestBuilder : BaseTestBuilder
{
    [PublicAPI]
    public ArrangeStepBuilder<T> Arrange<T>(Func<T> handler) => new(handler);
}