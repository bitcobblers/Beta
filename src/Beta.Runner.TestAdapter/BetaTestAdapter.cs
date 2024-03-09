using System.ComponentModel;
using Beta.Discovery;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Newtonsoft.Json;

namespace Beta.Runner.TestAdapter;

[PublicAPI]
[FileExtension(".dll")]
[FileExtension(".exe")]
[Category("managed")]
[DefaultExecutorUri(ExecutorUri)]
[ExtensionUri(ExecutorUri)]
public class BetaTestAdapter : ITestDiscoverer, ITestExecutor
{
    private const string ExecutorUri = "executor://Beta";

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public static TestProperty TestCaseProperty { get; } =
        TestProperty.Register("BetaTestCase", "Beta Test Case", typeof(string), typeof(BetaTestAdapter));

    public static TestProperty TestContainerProperty { get; } =
        TestProperty.Register("BetaTestContainer", "Beta Test Container", typeof(string), typeof(BetaTestAdapter));

    public static TestProperty TestMethodProper { get; } =
        TestProperty.Register("BetaTestMethod", "Beta Test Method", typeof(string), typeof(BetaTestAdapter));

    public void DiscoverTests(
        IEnumerable<string> sources,
        IDiscoveryContext discoveryContext,
        IMessageLogger logger,
        ITestCaseDiscoverySink discoverySink)
    {
        var logHelper = new InternalLogger(logger);
        var settings = RunSettings.Parse(discoveryContext.RunSettings?.SettingsXml);

        PrintBanner(logHelper, settings);

        foreach (var testCase in RunDiscovery(logHelper, sources, settings))
        {
            discoverySink.SendTestCase(testCase);
        }
    }

    public void RunTests(
        IEnumerable<string>? sources,
        IRunContext? runContext,
        IFrameworkHandle? frameworkHandle)
    {
        var logHelper = new InternalLogger(frameworkHandle);
        var settings = RunSettings.Parse(runContext?.RunSettings?.SettingsXml);

        PrintBanner(logHelper, settings);
        RunTests(RunDiscovery(logHelper, sources, settings), runContext, frameworkHandle);
    }

    public void RunTests(
        IEnumerable<TestCase>? tests,
        IRunContext? runContext,
        IFrameworkHandle? frameworkHandle)
    {
        var logHelper = new InternalLogger(frameworkHandle);
        var allTests = tests ?? Array.Empty<TestCase>();

        logHelper.Log(LogLevel.Info, "Running Tests");

        Parallel.ForEachAsync(allTests, _cancellationTokenSource.Token, async (test, token) =>
        {
            if (token.IsCancellationRequested)
            {
                frameworkHandle?.RecordResult(new TestResult(test)
                {
                    Outcome = TestOutcome.None
                });

                return;
            }

            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var outcome = TestOutcome.None;

            var input = test.GetPropertyValue<string>(TestCaseProperty, null);
            var containerType = test.GetPropertyValue<string>(TestContainerProperty, null);
            var method = test.GetPropertyValue<string>(TestMethodProper, null);

            if (containerType == null || method == null)
            {
                return;
            }

            var deserializedInput = input == null ? null : JsonConvert.DeserializeObject(input, options);

            var container = Activator.CreateInstance(Type.GetType(containerType)!) as TestContainer;
            var methodInfo = container?.GetType().GetMethod(method);
            var betaTest = methodInfo?.Invoke(container, null) as BetaTest;

            try
            {
                frameworkHandle?.RecordStart(test);

                container?.Prepare();
                _ = betaTest?.Apply(deserializedInput!);

                outcome = TestOutcome.Passed;
            }
            finally
            {
                frameworkHandle?.RecordEnd(test, outcome);
            }

            logHelper.Log(LogLevel.Info, $"Executing {test.FullyQualifiedName}");
            await Task.CompletedTask;
        }).Wait();
    }

    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
    }

    private static void PrintBanner(ICoreLogger logger, RunSettings settings)
    {
        logger.Log(LogLevel.Info, $"Target Framework Version: {settings.TargetFrameworkVersion}");
    }

    private IEnumerable<TestCase> RunDiscovery(
        ICoreLogger logger,
        IEnumerable<string>? sources,
        RunSettings settings)
    {
        var discoverer = new TestDiscoverer(logger);
        using var diaSessionManager = new DiaSessionManager();

        if (sources == null)
        {
            yield break;
        }

        foreach (var betaTest in discoverer.DiscoverTests(sources, settings.TargetFrameworkVersion))
        {
            var session = diaSessionManager.GetSession(betaTest.Assembly.Location);
            var navInfo = session.GetNavigationData(betaTest.Method);
            var fullyQualifiedName = betaTest.DeclaringType + "." + betaTest.MethodName;

            if (betaTest.Input is not null)
            {
                foreach (var input in betaTest.Input)
                {
                    var options = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };

                    var testCase = new TestCase
                    {
                        Id = Guid.NewGuid(),
                        CodeFilePath = navInfo?.FileName,
                        DisplayName = $"{betaTest.TestName}({input})",
                        ExecutorUri = new Uri(ExecutorUri),
                        FullyQualifiedName = fullyQualifiedName,
                        Source = betaTest.Assembly.Location,
                        LineNumber = navInfo?.MinLineNumber ?? 0
                    };

                    testCase.SetPropertyValue(TestCaseProperty, JsonConvert.SerializeObject(input, options));
                    testCase.SetPropertyValue(TestContainerProperty,
                        betaTest.Method?.DeclaringType?.AssemblyQualifiedName);
                    testCase.SetPropertyValue(TestMethodProper, betaTest.MethodName);

                    yield return testCase;
                }
            }
            else
            {
                var testCase = new TestCase
                {
                    Id = Guid.NewGuid(),
                    CodeFilePath = navInfo?.FileName,
                    DisplayName = betaTest.TestName,
                    ExecutorUri = new Uri(ExecutorUri),
                    FullyQualifiedName = fullyQualifiedName,
                    Source = betaTest.Assembly.Location,
                    LineNumber = navInfo?.MinLineNumber ?? 0
                };

                yield return testCase;
            }
        }
    }
}
