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
    private readonly ILogger _logger;
    private readonly Assembly _testAssembly;
    private readonly ITestAssemblyExplorer _testAssemblyExplorer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetaEngineController" /> class.
    /// </summary>
    /// <param name="testAssembly">The assembly to scan for tests in.</param>
    /// <param name="log">The logger callback to use.</param>
    public BetaEngineController(Assembly testAssembly, Action<int, string[], string, Exception?> log)
    {
        _logger = new InternalLogger(log);
        var serviceCollection = new ServiceCollection();

        _logger.Debug("Beginning controller initialization.");

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

        _logger.Debug("Controller initialization complete");
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetaEngineController" /> class.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="testAssembly">The assembly to scan for tests in.</param>
    /// <param name="testAssemblyExplorer">The assembly explorer implementation to use.</param>
    /// <remarks>
    ///     This constructor is only meant for unit testing.
    /// </remarks>
    internal BetaEngineController(ILogger logger,
        Assembly testAssembly,
        ITestAssemblyExplorer testAssemblyExplorer)
    {
        _logger = logger;
        _testAssembly = testAssembly;
        _testAssemblyExplorer = testAssemblyExplorer;
    }

    /// <summary>
    /// Wraps an invocation within a try-catch block to log exceptions.
    /// </summary>
    /// <typeparam name="T">The type to return from the invocation.</typeparam>
    /// <param name="defaultValue">The default value to return if the method fails.</param>
    /// <param name="func">The callback to execute.</param>
    /// <param name="caller">The calling method.</param>
    /// <returns>The result of the function, or default value if it fails.</returns>
    internal T Execute<T>(
        T defaultValue,
        Func<T> func,
        [CallerMemberName] string caller = "")
    {
        try
        {
            _logger.Debug($"Executing controller function [{caller}].");
            return func();
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
    /// <returns>A collection of discovered tests encoded in JSON.</returns>
    public IEnumerable<string> Query() =>
        Execute([], () => from test in _testAssemblyExplorer.Explore(_testAssembly)
                          select JsonSerializer.Serialize(new DiscoveredTest
                          {
                              ClassName = test.TestClassName,
                              MethodName = test.Method.Name,
                              Input = test.Input ?? string.Empty,
                              InputIndex = test.InputIndex,
                              TestName = test.FriendlyName
                          }));
}
