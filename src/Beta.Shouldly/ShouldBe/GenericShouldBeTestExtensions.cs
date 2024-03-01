using JetBrains.Annotations;
using Shouldly;

namespace Beta.Shouldly.ShouldBe;

[PublicAPI]
public static class GenericShouldBeTestExtensions
{
    public static Axiom<T> ShouldBe<T>(this Axiom<T> axiom, T? expected, string? customMessage = null)
    {
        return axiom.CallShouldly(actual => actual.ShouldBe(expected, customMessage));
    }

    public static Axiom<T> ShouldBe<T>(
        this Axiom<T> axiom, T? expected,
        IEqualityComparer<T> comparer,
        string? customMessage = null)
    {
        return axiom.CallShouldly(actual => actual.ShouldBe(expected, comparer, customMessage));
    }

    public static Axiom<T> ShouldNotBe<T>(this Axiom<T> axiom, T? expected, string? customMessage = null)
    {
        return axiom.CallShouldly(actual => actual.ShouldNotBe(expected, customMessage));
    }
}