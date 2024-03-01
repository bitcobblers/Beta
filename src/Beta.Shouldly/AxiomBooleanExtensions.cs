using System.Diagnostics.CodeAnalysis;
using Beta.Shouldly.ShouldBe;
using JetBrains.Annotations;
using Shouldly;

namespace Beta.Shouldly;

[PublicAPI]
public static class AxiomBooleanExtensions
{
    public static Axiom<bool> ShouldBeTrue([DoesNotReturnIf(false)] this Axiom<bool> axiom, string? customMessage = null)
    {
        return axiom.CallShouldly(actual => actual.ShouldBeTrue(customMessage));
    }

    public static Axiom<bool> ShouldBeFalse([DoesNotReturnIf(true)] this Axiom<bool> axiom,
        string? customMessage = null)
    {
        return axiom.CallShouldly(actual => actual.ShouldBeFalse(customMessage));
    }
}
