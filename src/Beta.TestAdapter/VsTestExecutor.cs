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
public class VsTestExecutor(EngineAdapterFactory? getAdapter, NavigationDataProviderFactory? getNavigation)
    : VsTestAdapter(getAdapter, getNavigation), ITestExecutor
{
    public const string ExecutorUri = "executor://BetaTestExecutor/v1";

    /// <summary>
    ///     Initializes a new instance of the <see cref="VsTestExecutor" /> class.
    /// </summary>
    [PublicAPI]
    public VsTestExecutor() :
        this(null, null)
    {
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public void RunTests(IEnumerable<TestCase>? tests,
                         IRunContext? runContext,
                         IFrameworkHandle? frameworkHandle)
    {
        Reset(runContext, frameworkHandle);

        if (frameworkHandle == null)
        {
            return;
        }

        var executionLogger = Logger.CreateScope("execution");
        executionLogger.Info("Executing tests.");

        foreach (var testGroup in from test in tests ?? []
                                  group test by test.Source
                                  into g
                                  select new
                                  {
                                      Source = g.Key,
                                      Tests = g.ToArray()
                                  })
        {
            foreach (var test in testGroup.Tests)
            {
                executionLogger.Debug($"Executing test: {test.FullyQualifiedName} ({test.DisplayName}).");
                frameworkHandle.RecordStart(test);
                frameworkHandle.RecordResult(new TestResult(test)
                {
                    Outcome = TestOutcome.Skipped
                });
                frameworkHandle.RecordEnd(test, TestOutcome.Skipped);
            }
        }
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public void RunTests(IEnumerable<string>? sources,
                         IRunContext? runContext,
                         IFrameworkHandle? frameworkHandle)
    {
        Reset(runContext, frameworkHandle);
        RunTests(CollectTests(sources ?? []), runContext, frameworkHandle);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public void Cancel()
    {
    }
}
