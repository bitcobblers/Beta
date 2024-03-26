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
    ///     Prints the current banner.
    /// </summary>
    protected void PrintBanner()
    {
        Logger.Info($"Target Framework Version: {Settings.Configuration.TargetFrameworkVersion}");
    }

    protected static TestCase ToTestCase(DiscoveredTest discoveredTest) =>
        new(discoveredTest.ClassName,
            new Uri(VsTestExecutor.ExecutorUri),
            "ignored")
        {
            FullyQualifiedName = $"{discoveredTest.ClassName}.{discoveredTest.MethodName}",
            DisplayName = discoveredTest.MethodName,
            CodeFilePath = "ignored",
            LineNumber = 0
        };
}
