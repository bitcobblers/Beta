using Beta.TestAdapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.Tests.TestAdapter;

public class TestLoggerTests
{
    public class Scoping : TestLoggerTests
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
    }

    public class LevelMapping : TestLoggerTests
    {
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

    public class ShouldLogMethod : TestLoggerTests
    {
        [InlineData(LogLevel.Debug, LogLevel.Debug, true)]
        [InlineData(LogLevel.Debug, LogLevel.Info, true)]
        [InlineData(LogLevel.Debug, LogLevel.Warn, true)]
        [InlineData(LogLevel.Debug, LogLevel.Error, true)]
        [InlineData(LogLevel.Info, LogLevel.Debug, false)]
        [InlineData(LogLevel.Info, LogLevel.Info, true)]
        [InlineData(LogLevel.Info, LogLevel.Warn, true)]
        [InlineData(LogLevel.Info, LogLevel.Error, true)]
        [InlineData(LogLevel.Warn, LogLevel.Debug, false)]
        [InlineData(LogLevel.Warn, LogLevel.Info, false)]
        [InlineData(LogLevel.Warn, LogLevel.Warn, true)]
        [InlineData(LogLevel.Warn, LogLevel.Error, true)]
        [InlineData(LogLevel.Error, LogLevel.Debug, false)]
        [InlineData(LogLevel.Error, LogLevel.Info, false)]
        [InlineData(LogLevel.Error, LogLevel.Warn, false)]
        [InlineData(LogLevel.Error, LogLevel.Error, true)]
        [Theory]
        public void CanDetermineIfMessageShouldBeLogged(LogLevel loggerLevel, LogLevel messageLevel, bool expected)
        {
            // Arrange.
            var logger = new TestLogger(A.Fake<IMessageLogger>(), loggerLevel);

            // Act.
            var actual = logger.ShouldLog(messageLevel);

            // Assert.
            actual.ShouldBe(expected);
        }
    }

    public class LogMethod : TestLoggerTests
    {
        [InlineData(LogLevel.Debug, LogLevel.Debug, false, 1)]
        [InlineData(LogLevel.Debug, LogLevel.Info, false, 1)]
        [InlineData(LogLevel.Debug, LogLevel.Warn, false, 1)]
        [InlineData(LogLevel.Debug, LogLevel.Warn, true, 2)]
        [InlineData(LogLevel.Debug, LogLevel.Error, false, 1)]
        [InlineData(LogLevel.Debug, LogLevel.Error, true, 2)]
        [InlineData(LogLevel.Info, LogLevel.Debug, false, 0)]
        [InlineData(LogLevel.Info, LogLevel.Info, false, 1)]
        [InlineData(LogLevel.Info, LogLevel.Warn, false, 1)]
        [InlineData(LogLevel.Info, LogLevel.Warn, true, 2)]
        [InlineData(LogLevel.Info, LogLevel.Error, false, 1)]
        [InlineData(LogLevel.Info, LogLevel.Error, true, 2)]
        [InlineData(LogLevel.Warn, LogLevel.Debug, false, 0)]
        [InlineData(LogLevel.Warn, LogLevel.Info, false, 0)]
        [InlineData(LogLevel.Warn, LogLevel.Warn, false, 1)]
        [InlineData(LogLevel.Warn, LogLevel.Warn, true, 2)]
        [InlineData(LogLevel.Warn, LogLevel.Error, false, 1)]
        [InlineData(LogLevel.Warn, LogLevel.Error, true, 2)]
        [InlineData(LogLevel.Error, LogLevel.Debug, false, 0)]
        [InlineData(LogLevel.Error, LogLevel.Info, false, 0)]
        [InlineData(LogLevel.Error, LogLevel.Warn, false, 0)]
        [InlineData(LogLevel.Error, LogLevel.Warn, true, 0)]
        [InlineData(LogLevel.Error, LogLevel.Error, false, 1)]
        [InlineData(LogLevel.Error, LogLevel.Error, true, 2)]
        [Theory]
        public void SendsMessageToLogger(LogLevel loggerLevel, LogLevel messageLevel, bool includeException,
                                         int callCount)
        {
            // Arrange.
            var ex = includeException ? new Exception() : null;
            var messageLogger = A.Fake<IMessageLogger>();
            var logger = new TestLogger(messageLogger, loggerLevel);

            // Act.
            logger.Log(messageLevel, "message", ex);

            // Assert.
            A.CallTo(() =>
                 messageLogger.SendMessage(A<TestMessageLevel>._, A<string>._))
             .MustHaveHappened(callCount, Times.Exactly);
        }
    }
}
