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

        foreach (var test in CollectTests(sources))
        {
            discoverySink.SendTestCase(test);
        }
    }
}
