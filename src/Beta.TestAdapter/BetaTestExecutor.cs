using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Beta.TestAdapter;

[ExtensionUri(ExecutorUri)]
public class BetaTestExecutor : BetaTestAdapter, ITestExecutor
{
    public const string ExecutorUri = "executor://Beta";

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <inheritdoc />
    public void RunTests(IEnumerable<TestCase>? tests, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
    {
        InitializeLogger(frameworkHandle);

        var allTests = tests ?? Array.Empty<TestCase>();

        Logger.Info("Running Tests");

        // Parallel.ForEachAsync(allTests, _cancellationTokenSource.Token, async (test, token) =>
        // {
        //     if (token.IsCancellationRequested)
        //     {
        //         frameworkHandle?.RecordResult(new TestResult(test)
        //         {
        //             Outcome = TestOutcome.None
        //         });
        //
        //         return;
        //     }
        //
        //     var options = new JsonSerializerSettings
        //     {
        //         TypeNameHandling = TypeNameHandling.All
        //     };
        //
        //     var outcome = TestOutcome.None;
        //
        //     var input = test.GetPropertyValue<string>(TestCaseProperty, null);
        //     var containerType = test.GetPropertyValue<string>(TestContainerProperty, null);
        //     var method = test.GetPropertyValue<string>(TestMethodProper, null);
        //
        //     if (containerType == null || method == null)
        //     {
        //         return;
        //     }
        //
        //     var deserializedInput = input == null ? null : JsonConvert.DeserializeObject(input, options);
        //
        //     var container = Activator.CreateInstance(Type.GetType(containerType)!) as TestContainer;
        //     var methodInfo = container?.GetType().GetMethod(method);
        //     var betaTest = methodInfo?.Invoke(container, null) as BetaTest;
        //
        //     try
        //     {
        //         frameworkHandle?.RecordStart(test);
        //
        //         container?.Prepare();
        //         _ = betaTest?.Apply(deserializedInput!);
        //
        //         outcome = TestOutcome.Passed;
        //     }
        //     finally
        //     {
        //         frameworkHandle?.RecordEnd(test, outcome);
        //     }
        //
        //     Logger.Info($"Executing {test.FullyQualifiedName}");
        //     await Task.CompletedTask;
        // }).Wait();
    }

    /// <inheritdoc />
    public void RunTests(IEnumerable<string>? sources, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
    {
        var settings = RunSettings.Parse(runContext?.RunSettings?.SettingsXml);

        InitializeLogger(frameworkHandle);
        PrintBanner(settings);
        RunTests(RunDiscovery(sources, settings), runContext, frameworkHandle);
    }

    /// <inheritdoc />
    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
    }
}
