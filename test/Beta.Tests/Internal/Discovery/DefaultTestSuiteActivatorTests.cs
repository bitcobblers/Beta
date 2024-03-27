using Beta.Internal.Discovery;
using Xunit.Abstractions;

namespace Beta.Tests.Internal.Discovery;

public class DefaultTestSuiteActivatorTests(ITestOutputHelper output)
{
    private readonly XUnitLogger _logger = new(output);

    [Fact]
    public void ReturnsNullWhenActivatorReturnsNonTestSuite()
    {
        // Arrange.
        var activator = new DefaultTestSuiteActivator(_logger);

        // Act.
        var result = activator.Create(typeof(NonTestSuite));

        // Assert.
        result.ShouldBeNull();
    }

    [Fact]
    public void ReturnsNullWhenActivatorReturnsNull()
    {
        // Arrange.
        var activator = new DefaultTestSuiteActivator(_logger);

        // Act.
        var result = activator.Create(typeof(TestSuiteThatThrowsException));

        // Assert.
        result.ShouldBeNull();
    }

    private class TestSuiteThatThrowsException : TestSuite
    {
        public TestSuiteThatThrowsException() => throw new Exception("I failed");
    }

    private class NonTestSuite;
}
