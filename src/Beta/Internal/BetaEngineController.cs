using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Beta.Internal.Discovery;
using Beta.Internal.Execution;
using Beta.Internal.Processors;
using Beta.Sdk.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Beta.Internal;

/// <summary>
///     Defines a controller that the adapter can invoke to perform test-related functions.
/// </summary>
[PublicAPI]
public class BetaEngineController
{
    private readonly bool _isInitialized;
    private readonly ILogger _logger;
    private readonly Assembly? _testAssembly;
    private readonly ITestAssemblyExplorer? _testAssemblyExplorer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetaEngineController" /> class.
    /// </summary>
    /// <param name="testAssembly">The assembly to scan for tests in.</param>
    /// <param name="log">The logger callback to use.</param>
    public BetaEngineController(Assembly testAssembly, Action<int, string, Exception?> log)
    {
        _logger = new InternalLogger(log);
        var serviceCollection = new ServiceCollection();

        _logger.Debug("Beginning controller initialization.");

        try
        {
            // TODO: Replace static discovery with dynamic discovery.
            serviceCollection.AddSingleton<ITestAssemblyExplorer, DefaultTestAssemblyExplorer>();
            serviceCollection.AddSingleton<ITestCaseDiscoverer, DefaultTestCaseDiscoverer>();
            serviceCollection.AddSingleton<ITestSuiteActivator, DefaultTestSuiteActivator>();
            serviceCollection.AddSingleton<ITestDiscoverer, DefaultTestDiscoverer>();
            serviceCollection.AddSingleton<ITestSuiteAggregator, DefaultTestSuiteAggregator>();
            serviceCollection.AddSingleton<ITestRunner, DefaultTestRunner>();
            serviceCollection.AddSingleton<ITestSuiteProcessor, InitializeContainerProcessor>();
            serviceCollection.AddSingleton(_logger);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _testAssembly = testAssembly;
            _testAssemblyExplorer = serviceProvider.GetRequiredService<ITestAssemblyExplorer>();
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred during controller initialization.", ex);
        }
        finally
        {
            _logger.Debug("Controller initialization complete.");
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetaEngineController" /> class.
    /// </summary>
    /// <param name="isInitialized">True if the controller should be marked as initialized.</param>
    /// <param name="logger">The logger to use.</param>
    /// <param name="testAssembly">The assembly to scan for tests in.</param>
    /// <param name="testAssemblyExplorer">The assembly explorer implementation to use.</param>
    /// <remarks>
    ///     This constructor is only meant for unit testing.
    /// </remarks>
    internal BetaEngineController(bool isInitialized, ILogger logger, Assembly? testAssembly,
                                  ITestAssemblyExplorer? testAssemblyExplorer)
    {
        _isInitialized = isInitialized;
        _logger = logger;
        _testAssembly = testAssembly;
        _testAssemblyExplorer = testAssemblyExplorer;
    }

    internal T ExecuteIfInitialized<T>(
        T defaultValue,
        Func<T> func,
        [CallerMemberName] string caller = "")
    {
        try
        {
            if (_isInitialized)
            {
                _logger.Debug($"Executing {caller}.");
                return func();
            }

            _logger.Error("Controller is not initialized.");
            return defaultValue;
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while executing a function.", ex);
            return defaultValue;
        }
    }

    /// <summary>
    ///     Queries the test assembly for all tests.
    /// </summary>
    /// <returns>A collection of discovered tests encoded in XML.</returns>
    public IEnumerable<string> Query() =>
        ExecuteIfInitialized([], () =>
            from test in _testAssemblyExplorer!.Explore(_testAssembly!)
            select JsonSerializer.Serialize(new DiscoveredTest
            {
                ClassName = test.TestClassName,
                MethodName = test.Method.Name,
                Input = test.Input ?? string.Empty,
                TestName = string.Empty
            }));
}
