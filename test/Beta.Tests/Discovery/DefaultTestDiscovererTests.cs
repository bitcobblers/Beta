using System.Diagnostics.CodeAnalysis;
using Beta.Discovery;

namespace Beta.Tests.Discovery;

public class DefaultTestDiscovererTests
{
    [Fact]
    public void ReturnsSingleTest()
    {
        // Arrange
        var discoverer = new DefaultTestDiscoverer();

        // Act
        var tests = discoverer.Discover(typeof(StubWithTest));

        // Assert
        tests.ShouldHaveSingleItem();
    }

    [Fact]
    public void ReturnsNoTests()
    {
        // Arrange
        var discoverer = new DefaultTestDiscoverer();

        // Act
        var tests = discoverer.Discover(typeof(StubWithNoTests)).ToArray();

        // Assert
        tests.ShouldBeEmpty();
    }

    private class StubWithTest : TestContainer
    {
        public BetaTest SingleTest()
        {
            return Test(() => new Proof<int>(42));
        }
    }

    private class StubWithNoTests : TestContainer
    {
        [UsedImplicitly]
        [SuppressMessage("Performance", "CA1822:Mark members as static")]
        public void NotATest()
        {
        }
    }
}
