using FakeItEasy;

namespace Beta.FakeItEasy;

[PublicAPI]
public static class ProofExtensions
{
    public static Proof<T> AssertFake<T>(this Proof<T> proof, Action<T> assertion)
    {
        proof.Assert(t =>
        {
            try
            {
                assertion(t);
            }
            catch (ExpectationException ex)
            {
                return new ProofResult(t, false, ex.Message);
            }

            return new ProofResult(t, true, "");
        });

        return proof;
    }
}