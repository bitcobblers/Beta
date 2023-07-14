// ReSharper disable once CheckNamespace

using Beta.AAA;
using JetBrains.Annotations;

namespace Beta;

public static class VerbExtensions
{
    [PublicAPI]
    public static ArrangeStepBuilder Arrange(this StepBuilder stepBuilder, Action handler) => 
        new(handler);

    [PublicAPI]
    public static ArrangeStepBuilder<T> Arrange<T>(this StepBuilder stepBuilder, Func<T> handler) => 
        new(handler);

    [PublicAPI]
    public static void Basic(this TestBuilder testBuilder, Action handler) => 
        testBuilder.AddSteps(_ => new BasicStepBuilder(handler));
}