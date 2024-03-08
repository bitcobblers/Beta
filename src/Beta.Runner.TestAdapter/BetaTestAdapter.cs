using System.ComponentModel;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.Runner.TestAdapter;

[FileExtension(".dll")]
[FileExtension(".exe")]
[Category("managed")]
[DefaultExecutorUri("executor://Beta")]
[ExtensionUri("executor://Beta")]
public class BetaTestAdapter(IAssemblySourceFilter sourceFilter)
    : ITestDiscoverer, ITestExecutor
{
    public BetaTestAdapter()
        : this(
            new DefaultAssemblySourceFilter(
                new NetCoreFrameworkMatcher()))
    {
        System.Diagnostics.Debugger.Launch();
        System.Diagnostics.Debugger.Break();
    }

    public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger,
                              ITestCaseDiscoverySink discoverySink)
    {
        var logHelper = new InternalLogger(logger);
        var settings = RunSettings.Parse(discoveryContext.RunSettings?.SettingsXml);
        PrintBanner(logHelper, settings);

        foreach (var source in sources)
        {
            foreach (var testCase in DiscoverTests(logHelper, source, settings))
            {
                logHelper.Log(LogLevel.Info, "*** BETA *** Discovered Test " + testCase.FullyQualifiedName);
                discoverySink.SendTestCase(testCase);
            }
        }
    }

    public void RunTests(IEnumerable<TestCase>? tests, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
    {
        var logHelper = new InternalLogger(frameworkHandle);
        var settings = RunSettings.Parse(runContext?.RunSettings?.SettingsXml);

        logHelper.Log(LogLevel.Info, "*** BETA *** Running Tests");

        foreach (var test in tests ?? Array.Empty<TestCase>())
        {
            logHelper.Log(LogLevel.Info, "*** BETA *** Executing {0}", test.FullyQualifiedName);
        }

        logHelper.Log(LogLevel.Info, "*** BETA *** Running Tests");
    }

    public void RunTests(IEnumerable<string>? sources, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
    {
        var logHelper = new InternalLogger(frameworkHandle);
        var settings = RunSettings.Parse(runContext?.RunSettings?.SettingsXml);
        PrintBanner(logHelper, settings);

        foreach (var source in sources ?? Array.Empty<string>())
        {
            RunTests(DiscoverTests(logHelper, source, settings), runContext, frameworkHandle);
        }
    }

    public void Cancel()
    {
    }

    private static void PrintBanner(InternalLogger logger, RunSettings settings)
    {
        logger.Log(LogLevel.Info,
            "*** BETA *** Target Framework Version: " + settings.TargetFrameworkVersion);
    }

    private IEnumerable<TestCase> DiscoverTests(InternalLogger logger, string source, RunSettings settings)
    {
        logger.Log(LogLevel.Info, "*** BETA *** Discovering Tests in " + source);

        try
        {
            if (sourceFilter.ShouldInclude(source, settings))
            {
                foreach (var testCase in ScanAssembly(logger, source, settings))
                {
                    yield return testCase;
                }
            }
        }
        finally
        {
            logger.Log(LogLevel.Info, "*** BETA *** Discovered Tests in " + source);
        }
    }

    public IEnumerable<TestCase> ScanAssembly(InternalLogger logger, string assemblyPath, RunSettings settings)
    {
        logger.Log(LogLevel.Info, "*** BETA *** Scanning Assembly " + assemblyPath);

        try
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            using var session = new DiaSessionWrapper(assemblyPath);

            foreach (var test in from type in assembly.GetTypes()
                                 where type.IsPublic && type.IsClass && type.IsAssignableTo(typeof(TestContainer))
                                 where type.GetConstructors().Any(c => c.GetParameters().Length == 0)
                                 let instance = Activator.CreateInstance(type) as TestContainer
                                 select instance.Discover())
            {
                foreach (var testCase in test)
                {
                    var navInfo = session.GetNavigationData(testCase.Method);
                    var fullyQualifiedName = testCase.Method?.DeclaringType?.FullName + "." + testCase.Method?.Name;

                    yield return new TestCase
                    {
                        Id = testCase.Id,
                        CodeFilePath = navInfo?.FileName,
                        DisplayName = testCase.Name,
                        ExecutorUri = new Uri("executor://Beta"),
                        FullyQualifiedName = fullyQualifiedName,
                        Source = assemblyPath,
                        LineNumber = navInfo?.MinLineNumber ?? 0,
                    };
                }
            }
        }
        finally
        {
            logger.Log(LogLevel.Info, "*** BETA *** Scanned Assembly " + assemblyPath);
        }
    }
}
