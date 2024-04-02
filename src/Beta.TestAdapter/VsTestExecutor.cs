using System.Diagnostics.CodeAnalysis;
using Beta.TestAdapter.Models;
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

    private readonly CancellationTokenSource _cancellationTokenSource = new();

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

        if (frameworkHandle is null)
        {
            return;
        }

        var testCollections = from test in tests ?? []
                              group test by test.Source
                              into g
                              let adapter = GetAdapter(g.Key)
                              let controller = adapter.GetController()
                              where controller is not null
                              select new TestCollection(controller, [.. g]);

        RunInternal(testCollections);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public void RunTests(IEnumerable<string>? sources,
                         IRunContext? runContext,
                         IFrameworkHandle? frameworkHandle)
    {
        Reset(runContext, frameworkHandle);

        if (frameworkHandle is null)
        {
            return;
        }

        RunInternal(CollectTests(sources ?? []));
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public void Cancel() =>
        _cancellationTokenSource.Cancel();

    internal void RunInternal(
        IEnumerable<TestCollection> testCollections)
    {
        var executionLogger = Logger.CreateScope("execution");

        foreach (var testGroup in testCollections)
        {
            foreach (var test in testGroup.Tests)
            {
                executionLogger.Debug($"Executing test: {test.FullyQualifiedName} ({test.DisplayName}).");

                var nameParts = test.FullyQualifiedName.Split('.');

                var testInvocation = new TestInvocationRequest
                {
                    ClassName = string.Join('.', nameParts.Take(nameParts.Length - 1)),
                    MethodName = nameParts.Last(),
                    InputIndex = test.GetPropertyValue(TestCaseInputProperty, 0)
                };

                testGroup.Controller.Run(testInvocation);
            }
        }
    }
}
