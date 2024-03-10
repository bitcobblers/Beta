using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Beta.Runner.TestAdapter;

public class BetaTestAdapter
{
    public static TestProperty TestCaseProperty { get; } =
        TestProperty.Register("BetaTestCase", "Beta Test Case", typeof(string), typeof(BetaTestAdapter));

    public static TestProperty TestContainerProperty { get; } =
        TestProperty.Register("BetaTestContainer", "Beta Test Container", typeof(string), typeof(BetaTestAdapter));

    public static TestProperty TestMethodProper { get; } =
        TestProperty.Register("BetaTestMethod", "Beta Test Method", typeof(string), typeof(BetaTestAdapter));

    protected static void PrintBanner(ICoreLogger logger, RunSettings settings)
    {
        logger.Log(LogLevel.Info, $"Target Framework Version: {settings.TargetFrameworkVersion}");
    }
}
