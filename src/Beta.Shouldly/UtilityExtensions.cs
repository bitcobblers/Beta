using Shouldly;

namespace Beta.Shouldly;

public static class UtilityExtensions
{
    internal static Axiom<T> CallShouldly<T>(this Axiom<T> axiom, Action<T> action) =>
        axiom.Assert(actual =>
        {
            try
            {
                action(actual);
                return new ProofResult(actual, true, string.Empty);
            }
            catch (ShouldAssertException ex)
            {
                return new ProofResult(actual, false, ex.Message);
            }
        });
}
