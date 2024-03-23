using Beta.TestAdapter.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Beta.TestAdapter;

/// <summary>
///     Defines a factory method that can resolve an adapter to the engine for a given assembly.
/// </summary>
/// <param name="assemblyPath">The path to the assembly to create the adapter for.</param>
public delegate IEngineAdapter AdapterFactory(string assemblyPath);

/// <summary>
///     Defines the base class for a test adapter.
/// </summary>
public class VsTestAdapter
{
    private readonly AdapterFactory _adapterFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VsTestAdapter" /> class.
    /// </summary>
    /// <param name="getAdapter">An optional factory method to get a new adapter.</param>
    internal VsTestAdapter(AdapterFactory? getAdapter)
    {
        _adapterFactory = getAdapter ?? (path => new BetaEngineAdapter(path, Logger));
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
    ///     Prints the current banner.
    /// </summary>
    protected void PrintBanner()
    {
        Logger.Info($"Target Framework Version: {Settings.Configuration.TargetFrameworkVersion}");
    }
}
