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
[DefaultExecutorUri(BetaTestExecutor.ExecutorUri)]
public class BetaTestDiscoverer : BetaTestAdapter, ITestDiscoverer
{
    /// <inheritdoc />
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

    public static IEnumerable<TestCase> RunDiscovery(
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
                        ExecutorUri = new Uri(BetaTestExecutor.ExecutorUri),
                        FullyQualifiedName = fullyQualifiedName,
                        Source = betaTest.Assembly.Location,
                        LineNumber = navInfo?.MinLineNumber ?? 0
                    };

                    testCase.SetPropertyValue(TestCaseProperty,
                        JsonConvert.SerializeObject(input, options));
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
                    ExecutorUri = new Uri(BetaTestExecutor.ExecutorUri),
                    FullyQualifiedName = fullyQualifiedName,
                    Source = betaTest.Assembly.Location,
                    LineNumber = navInfo?.MinLineNumber ?? 0
                };

                yield return testCase;
            }
        }
    }
}
