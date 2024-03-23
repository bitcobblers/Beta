using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Beta.TestAdapter;

/// <summary>
///     Defines an executor for beta tests.
/// </summary>
/// <param name="getAdapter">An optional factory method to create adapters with.</param>
[ExtensionUri(ExecutorUri)]
public class VsTestExecutor(AdapterFactory? getAdapter) : VsTestAdapter(getAdapter), ITestExecutor
{
    public const string ExecutorUri = "executor://BetaTestExecutor/v1";

    /// <summary>
    ///     Initializes a new instance of the <see cref="VsTestExecutor" /> class.
    /// </summary>
    [PublicAPI]
    public VsTestExecutor() :
        this(null)
    {
    }

    /// <inheritdoc />
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
}
