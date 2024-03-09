namespace Beta.Tests;

public class ProofTests
{
    [Fact]
    public async Task SinglePositiveAssertionReturnsSuccessResult()
    {
        // Arrange.
        var proof = new Proof<bool>(true);
        proof.Assert(actual => new ProofResult(actual, true, "It's true"));

        // Act.
        var results = await proof.Test().ToListAsync();

        // Assert.
        results.ShouldHaveSingleItem();
        results[0].ShouldBe(new ProofResult(true, true, "It's true"));
    }

    [Fact]
    public void Foo()
    {
    }

    //[Fact]
    //public void Bar()
    //{
    //}
}
