using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Beta.Internal.Discovery;
using Beta.Internal.Execution;
using Beta.Internal.Processors;
using Beta.Sdk.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Beta.Internal;

[PublicAPI]
public class BetaEngineController
{
    private readonly bool _isInitialized;
    private readonly ILogger _logger;
    private readonly Assembly? _testAssembly;
    private readonly ITestAssemblyExplorer? _testAssemblyExplorer;

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

    private T ExecuteIfInitialized<T>(
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
