using System.ComponentModel;
using System.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.Runner.TestAdapter;

[FileExtension(".dll")]
[FileExtension(".exe")]
[Category("managed")]
[DefaultExecutorUri(Constants.ExecutorUri)]
[ExtensionUri(Constants.ExecutorUri)]
public class BetaTestAdapter(IFrameworkMatcher frameworkMatcher)
    : ITestDiscoverer, ITestExecutor
{
    private static readonly HashSet<string> PlatformAssemblies = new(StringComparer.OrdinalIgnoreCase)
    {
        "microsoft.visualstudio.testplatform.unittestframework.dll",
        "microsoft.visualstudio.testplatform.core.dll",
        "microsoft.visualstudio.testplatform.testexecutor.core.dll",
        "microsoft.visualstudio.testplatform.extensions.msappcontaineradapter.dll",
        "microsoft.visualstudio.testplatform.objectmodel.dll",
        "microsoft.visualstudio.testplatform.utilities.dll",
        "vstest.executionengine.appcontainer.exe",
        "vstest.executionengine.appcontainer.x86.exe",

        "beta.testadapter.dll"
    };

    public BetaTestAdapter() : this(new NetCoreFrameworkMatcher())
    {
    }


    public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
    {
        System.Diagnostics.Debugger.Launch();
        System.Diagnostics.Debugger.Break();

        var logHelper = new InternalLogger(logger, Stopwatch.StartNew());
        var settings = RunSettings.Parse(discoveryContext.RunSettings?.SettingsXml);

        logHelper.Log(TestMessageLevel.Informational, "*** BETA *** Target Framework Version: " + settings.TargetFrameworkVersion);

        if (frameworkMatcher.IsMatch(settings.TargetFrameworkVersion) == false)
        {
            return;
        }
    }

    public void RunTests(IEnumerable<TestCase>? tests, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
    {
        System.Diagnostics.Debugger.Launch();
        System.Diagnostics.Debugger.Break();

        frameworkHandle!.SendMessage(TestMessageLevel.Informational, "*** BETA *** Running Tests");
    }

    public void RunTests(IEnumerable<string>? sources, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
    {
        System.Diagnostics.Debugger.Launch();
        System.Diagnostics.Debugger.Break();

        frameworkHandle!.SendMessage(TestMessageLevel.Informational, "*** BETA *** Starting Tests");
    }

    public void Cancel()
    {
    }
}