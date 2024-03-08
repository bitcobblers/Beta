using Beta.Runner.TestAdapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.Tests.TestAdapter;

public class InternalLoggerTests
{
    [Fact]
    public void DefaultScopeIsSlash()
    {
        // Arrange.
        var logger = new InternalLogger(null);

        // Assert.
        logger.Scope.ShouldBe("/");
    }

    [Fact]
    public void SingleChildScopeIsFormattedCorrectly()
    {
        // Arrange.
        var logger = new InternalLogger(null);

        // Act.
        var child = logger.CreateScope("child");

        // Assert.
        child.Scope.ShouldBe("/child");
    }

    [Fact]
    public void MultipleChildScopesAreFormattedCorrectly()
    {
        // Arrange.
        var logger = new InternalLogger(null);

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
        var actual = InternalLogger.ToTestMessageLevel(level);

        // Assert.
        actual.ShouldBe(expected);
    }
}
