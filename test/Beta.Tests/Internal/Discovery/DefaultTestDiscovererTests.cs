using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Beta.Internal.Discovery;
using Beta.Sdk.Abstractions;
using Beta.Sdk.Interfaces;

namespace Beta.Tests.Internal.Discovery;

public class DefaultTestDiscovererTests
{
    [Fact]
    public void ReturnsSingleTest()
    {
        // Arrange
        var testCaseDiscoverer = A.Fake<ITestCaseDiscoverer>();
        var discoverer = new DefaultTestDiscoverer(testCaseDiscoverer);

        A.CallTo(() =>
             testCaseDiscoverer.Discover(A<MethodInfo>._))
         .Returns(new[] { A.Dummy<Test>() });

        // Act
        var tests = discoverer.Discover(typeof(StubWithTest)).ToArray();

        // Assert
        tests.ShouldHaveSingleItem();
    }

    [Fact]
    public void ReturnsNoTests()
    {
        // Arrange
        var testCaseDiscoverer = A.Fake<ITestCaseDiscoverer>();
        var discoverer = new DefaultTestDiscoverer(testCaseDiscoverer);

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