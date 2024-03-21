using System.ComponentModel;
using System.Diagnostics;
using Beta.TestAdapter.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter;

[FileExtension("*.dll")]
[FileExtension("*.exe")]
[DefaultExecutorUri(ExecutorUri)]
[Category("managed")]
[ExtensionUri(ExecutorUri)]
public class VsTestDiscoverer : ITestDiscoverer, ITestExecutor
{
    public const string ExecutorUri = "executor://BetaTestExecutor/v1";

    public VsTestDiscoverer()
    {
        System.Diagnostics.Debugger.Launch();
        System.Diagnostics.Debugger.Break();
    }

    /// <inheritdoc />
    public void DiscoverTests(IEnumerable<string> sources,
                              IDiscoveryContext discoveryContext,
                              IMessageLogger logger,
                              ITestCaseDiscoverySink discoverySink)
    {
        Reset(discoveryContext, logger);
        PrintBanner();

        foreach (var source in sources)
        {
            var assemblyPath = Path.IsPathRooted(source)
                ? source
                : Path.Combine(Directory.GetCurrentDirectory(), source);

            var engineAdapter = new BetaEngineAdapter(assemblyPath, Logger);

            foreach (var testCase in engineAdapter.Query())
            {
                discoverySink.SendTestCase(testCase);
            }
        }
    }

    public void RunTests(IEnumerable<TestCase>? tests,
        IRunContext? runContext,
        IFrameworkHandle? frameworkHandle)
    {
        Reset(runContext, frameworkHandle);
        PrintBanner();
    }

    /// <inheritdoc />
    public void RunTests(IEnumerable<string>? sources,
        IRunContext? runContext,
        IFrameworkHandle? frameworkHandle)
    {
        Reset(runContext, frameworkHandle);
        PrintBanner();
    }

    /// <inheritdoc />
    public void Cancel()
    {
    }

    /// <summary>
    ///     Gets the internal logger.
    /// </summary>
    protected ITestLogger Logger { get; private set; } = TestLogger.Null;

    /// <summary>
    ///     Gets the run settings for the adapter.
    /// </summary>
    protected RunSettings Settings { get; private set; } = new();

    /// <summary>
    ///     Resets the adapter with the given context.
    /// </summary>
    /// <param name="context">The context to reset with.</param>
    /// <param name="logger">The current logger.</param>
    protected void Reset(IDiscoveryContext? context, IMessageLogger? logger)
    {
        Settings = RunSettings.Parse(context?.RunSettings?.SettingsXml);
        Logger = new TestLogger(logger);
    }

    /// <summary>
    ///     Prints the current banner.
    /// </summary>
    protected void PrintBanner()
    {
        Logger.Info($"Target Framework Version: {Settings.Configuration.TargetFrameworkVersion}");
    }
}
