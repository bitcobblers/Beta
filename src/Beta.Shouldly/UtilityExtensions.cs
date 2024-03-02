using Shouldly;

namespace Beta.Shouldly;

public static class UtilityExtensions
{
    internal static Proof<T> CallShouldly<T>(this Proof<T> proof, Action<T> action) =>
        proof.Assert(actual =>
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
