using Beta.Internal.Discovery;

namespace Beta.Tests.Internal.Discovery;

public class DefaultTestSuiteActivatorTests
{
    [Fact]
    public void ThrowsTestSuiteActivationFailedExceptionWhenActivatorReturnsNonTestSuite()
    {
        // Arrange.
        var activator = new DefaultTestSuiteActivator();

        // Assert.
        Assert.Throws<TestSuiteActivationFailedException>(() =>
            activator.Create(typeof(DefaultTestSuiteActivatorTests)));
    }

    [Fact]
    public void ThrowsTestSuiteActivationFailedExceptionWhenActivatorReturnsNull()
    {
        // Arrange.
        var activator = new DefaultTestSuiteActivator();

        // Assert.
        Assert.Throws<TestSuiteActivationFailedException>(() =>
            activator.Create(typeof(TestSuiteThatThrowsException)));
    }

    private class TestSuiteThatThrowsException : TestSuite
    {
        public TestSuiteThatThrowsException() => throw new Exception("I failed");
    }
}
