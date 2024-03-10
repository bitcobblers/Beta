using System.ComponentModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter;

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
        var settings = RunSettings.Parse(discoveryContext.RunSettings?.SettingsXml);

        Initialize(discoveryContext, logger);
        PrintBanner(settings);

        foreach (var testCase in RunDiscovery(sources, settings))
        {
            discoverySink.SendTestCase(testCase);
        }
    }
}
