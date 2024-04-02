using Beta.TestAdapter.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using static Beta.TestAdapter.Factories;

namespace Beta.TestAdapter;

/// <summary>
///     Defines the base class for a test adapter.
/// </summary>
public class VsTestAdapter
{
    private readonly EngineAdapterFactory _adapterFactory;
    private readonly NavigationDataProviderFactory _navigationFactory;

    /// <summary>
    ///    The property to store the test case input.
    /// </summary>
    internal static readonly TestProperty TestCaseInputProperty =
        TestProperty.Register(
            "Beta.TestCaseInput",
            "Input data for the test case",
            typeof(int),
            typeof(VsTestAdapter));

    /// <summary>
    ///     Initializes a new instance of the <see cref="VsTestAdapter" /> class.
    /// </summary>
    /// <param name="getAdapter">An optional factory method to get a new adapter.</param>
    /// <param name="getNavigation">An optional factory method to get a new data navigator.</param>
    internal VsTestAdapter(EngineAdapterFactory? getAdapter, NavigationDataProviderFactory? getNavigation)
    {
        _adapterFactory = getAdapter ?? (path => new BetaEngineAdapter(path, Logger));
        _navigationFactory = getNavigation ?? (path => new NavigationDataProvider(path));
    }

    /// <summary>
    ///     Gets the internal logger.
    /// </summary>
    protected ITestLogger Logger { get; private set; } = TestLogger.Null;

    /// <summary>
    ///     Gets the run settings for the adapter.
    /// </summary>
    protected RunSettings Settings { get; private set; } = new();

    /// <summary>
    ///     Resets the adapter with the given context.
    /// </summary>
    /// <param name="context">The context to reset with.</param>
    /// <param name="logger">The current logger.</param>
    protected void Reset(IDiscoveryContext? context, IMessageLogger? logger)
    {
        Settings = RunSettings.Parse(context?.RunSettings?.SettingsXml);
        Logger = new TestLogger(logger);
    }

    /// <summary>
    ///     Gets an adapter to the engine for the given assembly path.
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly to get the adapter for.</param>
    /// <returns>A new adapter for the engine.</returns>
    protected IEngineAdapter GetAdapter(string assemblyPath) =>
        _adapterFactory(assemblyPath);

    /// <summary>
    ///     Gets the navigation data provider for the given assembly path.
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly to read.</param>
    /// <returns>A new navigation provider for the assembly.</returns>
    protected INavigationDataProvider GetNavigation(string assemblyPath) =>
        _navigationFactory(assemblyPath);

    /// <summary>
    ///     Converts a discovered test from the controller into a <see cref="TestCase" /> usable by vstest.
    /// </summary>
    /// <param name="discoveredTest">The discovered test.</param>
    /// <param name="source">The path to the source assembly the test came from.</param>
    /// <param name="navData">The navigation data used to resolve metadata from.</param>
    /// <returns>The converted <see cref="TestCase" />.</returns>
    internal static TestCase ToTestCase(
        DiscoveredTest discoveredTest,
        string source,
        INavigationDataProvider navData)
    {
        var sourceInformation = navData.Get(discoveredTest.ClassName, discoveredTest.MethodName);

        var testCase = new TestCase(
            discoveredTest.ClassName,
            new Uri(VsTestExecutor.ExecutorUri),
            source)
        {
            Id = Guid.NewGuid(),
            FullyQualifiedName = $"{discoveredTest.ClassName}.{discoveredTest.MethodName}",
            DisplayName = discoveredTest.TestName,
            CodeFilePath = sourceInformation?.FileName,
            LineNumber = sourceInformation?.LineNumber ?? 1
        };

        testCase.SetPropertyValue(TestCaseInputProperty, discoveredTest.InputIndex);

        return testCase;
    }

    /// <summary>
    ///     Collects a collection of tests from the given sources.
    /// </summary>
    /// <param name="sources">The sources to look for tests in.</param>
    /// <returns>A collection of tests.</returns>
    protected IEnumerable<TestCase> CollectTests(IEnumerable<string> sources)
    {
        var adapterLogger = Logger.CreateScope("adapter");

        adapterLogger.Debug("Collecting tests.");

        foreach (var source in sources)
        {
            var assemblyPath = Path.IsPathRooted(source)
                ? source
                : Path.Combine(Directory.GetCurrentDirectory(), source);

            var engineAdapter = GetAdapter(assemblyPath);
            var controller = engineAdapter.GetController();
            using var navigation = GetNavigation(assemblyPath);

            if (controller is null)
            {
                adapterLogger.Error($"Failed to get controller for [{assemblyPath}].");
                continue;
            }

            foreach (var testCase in from test in controller.Query()
                                         // ReSharper disable once AccessToDisposedClosure
                                     select ToTestCase(test, source, navigation))
            {
                adapterLogger.Debug($"Discovered test [{testCase.DisplayName}].");
                yield return testCase;
            }
        }
    }
}
