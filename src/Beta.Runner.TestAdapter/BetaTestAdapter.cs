using System.ComponentModel;
using Beta.Discovery;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

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

        logHelper.Log(LogLevel.Info, "Running Tests");

        foreach (var test in tests ?? Array.Empty<TestCase>())
        {
            logHelper.Log(LogLevel.Info, $"Executing {test.FullyQualifiedName}");
        }

        logHelper.Log(LogLevel.Info, "Running Tests");
    }

    public void Cancel()
    {
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
                    var testCase = new TestCase
                    {
                        Id = Guid.NewGuid(),
                        CodeFilePath = navInfo?.FileName,
                        DisplayName = $"{betaTest.TestName}({input})",
                        ExecutorUri = new Uri(ExecutorUri),
                        FullyQualifiedName = fullyQualifiedName,
                        Source = betaTest.Assembly.Location,
                        LineNumber = navInfo?.MinLineNumber ?? 0,
                    };

                    // testCase.SetPropertyValue("Input", input);
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
                    LineNumber = navInfo?.MinLineNumber ?? 0,
                };


                yield return testCase;
            }
        }
    }
}
