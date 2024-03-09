using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Beta.Runner.TestAdapter;

public static class BetaTestProperties
{
    public static TestProperty TestCaseProperty { get; } =
        TestProperty.Register("BetaTestCase", "Beta Test Case", typeof(string), typeof(BetaTestProperties));

    public static TestProperty TestContainerProperty { get; } =
        TestProperty.Register("BetaTestContainer", "Beta Test Container", typeof(string), typeof(BetaTestProperties));

    public static TestProperty TestMethodProper { get; } =
        TestProperty.Register("BetaTestMethod", "Beta Test Method", typeof(string), typeof(BetaTestProperties));
}