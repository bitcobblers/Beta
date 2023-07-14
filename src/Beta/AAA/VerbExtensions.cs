// ReSharper disable once CheckNamespace
namespace Beta.AAA;

public static class VerbExtensions
{
    public static ArrangeStepBuilder Arrange(this StepBuilder stepBuilder, Action handler) => new (handler);
    public static ArrangeStepBuilder<T> Arrange<T>(this StepBuilder stepBuilder, Func<T> handler) => new(handler);
}