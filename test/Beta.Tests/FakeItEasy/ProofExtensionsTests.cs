using Beta.FakeItEasy;

namespace Beta.Tests.FakeItEasy;

public class ProofExtensionsTests
{
    [Fact]
    public async Task AssertFakeProducesFailureWhenExpectationNotMet()
    {
        // Arrange.
        var fake = A.Fake<IStubService>();
        var proof = new Proof<IStubService>(fake);

        proof.AssertFake(f => A.CallTo(() => f.DoWork()).MustHaveHappened());

        // Act.
        var results = await proof.Test().ToListAsync();

        // Assert.
        results.ShouldHaveSingleItem();
        results[0].Success.ShouldBeFalse();
    }

    [Fact]
    public async Task AssertFakeProducesSuccessWhenExpectationMet()
    {
        // Arrange.
        var fake = A.Fake<IStubService>();
        var proof = new Proof<IStubService>(fake);

        proof.AssertFake(f => A.CallTo(() => f.DoWork()).MustHaveHappened());

        // Act.
        fake.DoWork();
        var results = await proof.Test().ToListAsync();

        // Assert.
        results.ShouldHaveSingleItem();
        results[0].Success.ShouldBeTrue();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public interface IStubService
    {
        void DoWork();
    }
}