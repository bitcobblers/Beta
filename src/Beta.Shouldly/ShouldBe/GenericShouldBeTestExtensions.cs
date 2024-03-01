using Shouldly;

namespace Beta.Shouldly.ShouldBe;

[PublicAPI]
public static class GenericShouldBeTestExtensions
{
    public static Proof<T> ShouldBe<T>(this Proof<T> proof, T? expected, string? customMessage = null)
    {
        return proof.CallShouldly(actual => actual.ShouldBe(expected, customMessage));
    }

    public static Proof<T> ShouldBe<T>(
        this Proof<T> proof, T? expected,
        IEqualityComparer<T> comparer,
        string? customMessage = null)
    {
        return proof.CallShouldly(actual => actual.ShouldBe(expected, comparer, customMessage));
    }

    public static Proof<T> ShouldNotBe<T>(this Proof<T> proof, T? expected, string? customMessage = null)
    {
        return proof.CallShouldly(actual => actual.ShouldNotBe(expected, customMessage));
    }
}