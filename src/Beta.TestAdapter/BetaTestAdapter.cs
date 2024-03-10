using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter;

public class BetaTestAdapter
{
    public static TestProperty TestCaseProperty { get; } =
        TestProperty.Register("BetaTestCase", "Beta Test Case", typeof(string), typeof(BetaTestAdapter));

    public static TestProperty TestContainerProperty { get; } =
        TestProperty.Register("BetaTestContainer", "Beta Test Container", typeof(string), typeof(BetaTestAdapter));

    public static TestProperty TestMethodProper { get; } =
        TestProperty.Register("BetaTestMethod", "Beta Test Method", typeof(string), typeof(BetaTestAdapter));

    protected ITestLogger Logger { get; private set; } = new TestLogger();

    protected void InitializeLogger(IMessageLogger? logger, LogLevel verbosity = LogLevel.Info)
    {
        Logger = new TestLogger(logger, verbosity);
    }

    protected void PrintBanner(RunSettings settings)
    {
        Logger.Info($"Target Framework Version: {settings.TargetFrameworkVersion}");
    }

    protected IEnumerable<TestCase> RunDiscovery(
        IEnumerable<string>? sources,
        RunSettings settings)
    {
        yield break;

        // var discoverer = new TestDiscoverer(logger);
        // using var diaSessionManager = new DiaSessionManager();
        //
        // if (sources == null)
        // {
        //     yield break;
        // }
        //
        // foreach (var betaTest in discoverer.DiscoverTests(sources, settings.TargetFrameworkVersion))
        // {
        //     var session = diaSessionManager.GetSession(betaTest.Assembly.Location);
        //     var navInfo = session.GetNavigationData(betaTest.Method);
        //     var fullyQualifiedName = betaTest.DeclaringType + "." + betaTest.MethodName;
        //
        //     if (betaTest.Input is not null)
        //     {
        //         foreach (var input in betaTest.Input)
        //         {
        //             var options = new JsonSerializerSettings
        //             {
        //                 TypeNameHandling = TypeNameHandling.All
        //             };
        //
        //             var testCase = new TestCase
        //             {
        //                 Id = Guid.NewGuid(),
        //                 CodeFilePath = navInfo?.FileName,
        //                 DisplayName = $"{betaTest.TestName}({input})",
        //                 ExecutorUri = new Uri(BetaTestExecutor.ExecutorUri),
        //                 FullyQualifiedName = fullyQualifiedName,
        //                 Source = betaTest.Assembly.Location,
        //                 LineNumber = navInfo?.MinLineNumber ?? 0
        //             };
        //
        //             testCase.SetPropertyValue(TestCaseProperty, JsonConvert.SerializeObject(input, options));
        //             testCase.SetPropertyValue(TestContainerProperty,
        //                 betaTest.Method?.DeclaringType?.AssemblyQualifiedName);
        //             testCase.SetPropertyValue(TestMethodProper, betaTest.MethodName);
        //
        //             yield return testCase;
        //         }
        //     }
        //     else
        //     {
        //         var testCase = new TestCase
        //         {
        //             Id = Guid.NewGuid(),
        //             CodeFilePath = navInfo?.FileName,
        //             DisplayName = betaTest.TestName,
        //             ExecutorUri = new Uri(BetaTestExecutor.ExecutorUri),
        //             FullyQualifiedName = fullyQualifiedName,
        //             Source = betaTest.Assembly.Location,
        //             LineNumber = navInfo?.MinLineNumber ?? 0
        //         };
        //
        //         yield return testCase;
        //     }
        // }
    }
}
