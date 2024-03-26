using System.ComponentModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using static Beta.TestAdapter.Factories;

namespace Beta.TestAdapter;

/// <summary>
///     Defines the discoverer for beta tests.
/// </summary>
/// <param name="getAdapter">An optional factory method to create adapters with.</param>
[FileExtension(".dll")]
[FileExtension(".exe")]
[DefaultExecutorUri(VsTestExecutor.ExecutorUri)]
[Category("managed")]
public class VsTestDiscoverer(EngineAdapterFactory? getAdapter, NavigationDataProviderFactory? getNavigation)
    : VsTestAdapter(getAdapter, getNavigation), ITestDiscoverer
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="VsTestDiscoverer" /> class.
    /// </summary>
    [PublicAPI]
    public VsTestDiscoverer() :
        this(null, null)
    {
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

            var engineAdapter = GetAdapter(assemblyPath);
            var engine = engineAdapter.GetController();
            using var navigation = GetNavigation(assemblyPath);

            if (engine == null)
            {
                Logger.Error($"Failed to get controller for [{assemblyPath}].");
                continue;
            }

            foreach (var testCase in from test in engine.Query()
                                     select ToTestCase(test))
            {
                Logger.Debug($"Discovered test [{testCase.DisplayName}].");
                discoverySink.SendTestCase(testCase);
            }
        }
    }
}
