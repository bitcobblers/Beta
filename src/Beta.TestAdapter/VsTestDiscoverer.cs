﻿using System.ComponentModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter;

[FileExtension("*.dll")]
[FileExtension("*.exe")]
[DefaultExecutorUri(VsTestExecutor.ExecutorUri)]
[Category("managed")]
public class VsTestDiscoverer : VsTestAdapter, ITestDiscoverer
{
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

            foreach (var testCase in engineAdapter.Discover())
            {
                discoverySink.SendTestCase(testCase);
            }
        }
    }
}