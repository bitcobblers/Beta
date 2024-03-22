using System.Diagnostics.CodeAnalysis;
using Shouldly;

namespace Beta.Shouldly;

[PublicAPI]
public static class BooleanExtensions
{
    public static Proof<bool> ShouldBeTrue([DoesNotReturnIf(false)] this Proof<bool> proof,
                                           string? customMessage = null)
    {
        return proof.CallShouldly(actual => actual.ShouldBeTrue(customMessage));
    }

    public static Proof<bool> ShouldBeFalse([DoesNotReturnIf(true)] this Proof<bool> proof,
                                            string? customMessage = null)
    {
        return proof.CallShouldly(actual => actual.ShouldBeFalse(customMessage));
    }
}