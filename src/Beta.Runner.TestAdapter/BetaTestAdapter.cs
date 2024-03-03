namespace Beta.Runner.TestAdapter;
using System.ComponentModel;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

[FileExtension(".dll")]
[FileExtension(".exe")]
[Category("managed")]
[DefaultExecutorUri(Constants.ExecutorUri)]
[ExtensionUri(Constants.ExecutorUri)]
public class BetaTestAdapter : ITestDiscoverer, ITestExecutor
{
    public BetaTestAdapter()
    {
        System.Diagnostics.Debugger.Launch();
        System.Diagnostics.Debugger.Break();
    }

    public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
    {
        logger.SendMessage(TestMessageLevel.Informational, "*** BETA *** Attempting Discovery");
    }

    public void RunTests(IEnumerable<TestCase>? tests, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
    {
        frameworkHandle!.SendMessage(TestMessageLevel.Informational, "*** BETA *** Running Tests");
    }

    public void RunTests(IEnumerable<string>? sources, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
    {
        frameworkHandle!.SendMessage(TestMessageLevel.Informational, "*** BETA *** Starting Tests");
    }

    public void Cancel() { }
}