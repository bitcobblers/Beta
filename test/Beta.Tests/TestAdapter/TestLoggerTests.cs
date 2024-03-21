using Beta.TestAdapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.Tests.TestAdapter;

public class TestLoggerTests
{
    [Fact]
    public void DefaultScopeIsSlash()
    {
        // Arrange.
        var logger = new TestLogger(A.Fake<IMessageLogger>());

        // Assert.
        logger.Scope.ShouldBe("/");
    }

    [Fact]
    public void SingleChildScopeIsFormattedCorrectly()
    {
        // Arrange.
        var logger = new TestLogger(A.Fake<IMessageLogger>());

        // Act.
        var child = logger.CreateScope("child");

        // Assert.
        child.Scope.ShouldBe("/child");
    }

    [Fact]
    public void MultipleChildScopesAreFormattedCorrectly()
    {
        // Arrange.
        var logger = new TestLogger(A.Fake<IMessageLogger>());

        // Act.
        var child = logger.CreateScope("child");
        var grandchild = child.CreateScope("grandchild");

        // Assert.
        grandchild.Scope.ShouldBe("/child/grandchild");
    }

    [InlineData(LogLevel.Info, TestMessageLevel.Informational)]
    [InlineData(LogLevel.Warn, TestMessageLevel.Warning)]
    [InlineData(LogLevel.Error, TestMessageLevel.Error)]
    [Theory]
    public void CanMapLogLevelToTestMessageLevel(LogLevel level, TestMessageLevel expected)
    {
        // Arrange.
        var actual = TestLogger.ToTestMessageLevel(level);

        // Assert.
        actual.ShouldBe(expected);
    }
}