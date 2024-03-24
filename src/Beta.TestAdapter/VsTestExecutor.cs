using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using static Beta.TestAdapter.Factories;

namespace Beta.TestAdapter;

/// <summary>
///     Defines an executor for beta tests.
/// </summary>
/// <param name="getAdapter">An optional factory method to create adapters with.</param>
[ExtensionUri(ExecutorUri)]
public class VsTestExecutor(EngineAdapterFactory? getAdapter) : VsTestAdapter(getAdapter), ITestExecutor
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
    [ExcludeFromCodeCoverage]
    public void RunTests(IEnumerable<TestCase>? tests,
                         IRunContext? runContext,
                         IFrameworkHandle? frameworkHandle)
    {
        Reset(runContext, frameworkHandle);
        PrintBanner();
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public void RunTests(IEnumerable<string>? sources,
                         IRunContext? runContext,
                         IFrameworkHandle? frameworkHandle)
    {
        Reset(runContext, frameworkHandle);
        PrintBanner();
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public void Cancel()
    {
    }
}
