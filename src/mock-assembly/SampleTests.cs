using System.Diagnostics.CodeAnalysis;
using Beta;

namespace mock_assembly;

[PublicAPI]
[ExcludeFromCodeCoverage]
public class SampleTests : TestContainer
{
    public BetaTest SampleTest() =>
        Test(() =>
            from a in Gather(1)
            from b in Gather(2)
            let result = Apply(() => a + b)
            select result.Assert(x => new ProofResult(null, x == 3, "Sample Test")));
}
