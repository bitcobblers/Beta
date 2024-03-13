using Beta.Engine.Internal;

namespace Beta.Engine.Core.Tests.Internal.Logging;

public class LoggingTests
{
    public static IEnumerable<object[]> Levels
    {
        get
        {
            yield return [InternalTraceLevel.Error];
            yield return [InternalTraceLevel.Warning];
            yield return [InternalTraceLevel.Info];
            yield return [InternalTraceLevel.Debug];
        }
    }

    [InlineData(InternalTraceLevel.Error, InternalTraceLevel.Error)]
    [InlineData(InternalTraceLevel.Error, InternalTraceLevel.Warning)]
    [InlineData(InternalTraceLevel.Error, InternalTraceLevel.Info)]
    [InlineData(InternalTraceLevel.Error, InternalTraceLevel.Debug)]
    [InlineData(InternalTraceLevel.Warning, InternalTraceLevel.Error)]
    [InlineData(InternalTraceLevel.Warning, InternalTraceLevel.Warning)]
    [InlineData(InternalTraceLevel.Warning, InternalTraceLevel.Info)]
    [InlineData(InternalTraceLevel.Warning, InternalTraceLevel.Debug)]
    [InlineData(InternalTraceLevel.Info, InternalTraceLevel.Error)]
    [InlineData(InternalTraceLevel.Info, InternalTraceLevel.Warning)]
    [InlineData(InternalTraceLevel.Info, InternalTraceLevel.Info)]
    [InlineData(InternalTraceLevel.Info, InternalTraceLevel.Debug)]
    [InlineData(InternalTraceLevel.Debug, InternalTraceLevel.Error)]
    [InlineData(InternalTraceLevel.Debug, InternalTraceLevel.Warning)]
    [InlineData(InternalTraceLevel.Debug, InternalTraceLevel.Info)]
    [InlineData(InternalTraceLevel.Debug, InternalTraceLevel.Debug)]
    [Theory]
    public void LoggerSelectsMessagesToWrite(
        InternalTraceLevel logLevel,
        InternalTraceLevel msgLevel)
    {
        var writer = new StringWriter();
        var logger = new Logger("MyLogger", logLevel, writer);

        logger.TraceLevel.ShouldBe(logLevel);

        var msg = "This is my message";

        switch (msgLevel)
        {
            case InternalTraceLevel.Error:
                logger.Error(msg);
                break;
            case InternalTraceLevel.Warning:
                logger.Warning(msg);
                break;
            case InternalTraceLevel.Info:
                logger.Info(msg);
                break;
            case InternalTraceLevel.Debug:
                logger.Debug(msg);
                break;
        }

        var output = writer.ToString();

        if (logLevel >= msgLevel)
        {
            output.ShouldContain($" {msgLevel} ");
            output.ShouldEndWith($"MyLogger: {msg}" + Environment.NewLine);
        }
        else
        {
            output.ShouldBeEmpty();
        }
    }

    [Fact]
    public void GetLoggerWithDefaultTraceLevel()
    {
        var logger = InternalTrace.GetLogger("MyLogger");
        logger.TraceLevel.ShouldBe(InternalTrace.DefaultTraceLevel);
    }

    // [TestCaseSource(nameof(LEVELS))]
    [MemberData(nameof(Levels))]
    [Theory]
    public void GetLoggerWithSpecifiedTraceLevel(InternalTraceLevel level)
    {
        var logger = InternalTrace.GetLogger("MyLogger", level);
        logger.TraceLevel.ShouldBe(level);
    }
}
