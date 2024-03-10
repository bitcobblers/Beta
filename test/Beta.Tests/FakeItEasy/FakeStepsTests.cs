using Beta.FakeItEasy;
using static Beta.CommonSteps;

namespace Beta.Tests.FakeItEasy;

using static FakeSteps;

public class FakeStepsTests
{
    [Fact]
    public async Task CanCreateFakeWithExtensionMethod()
    {
        // Arrange.
        var proof = new TestContainerWithFake().TestWithFake();

        // Act.
        var result = await proof.Test().ToListAsync();

        // Assert.
        result.ShouldHaveSingleItem();
        result[0].Success.ShouldBeTrue();
    }

    private class TestContainerWithFake : TestContainer
    {
        public Proof<string> TestWithFake()
        {
            return
                from service in Fake<IStubService>(fake => A.CallTo(() => fake.DoWork()).Returns("expected"))
                let result = Apply(service.DoWork)
                select result.Assert(f => new ProofResult(f, f == "expected", string.Empty));
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public interface IStubService
    {
        string DoWork();
    }
}
