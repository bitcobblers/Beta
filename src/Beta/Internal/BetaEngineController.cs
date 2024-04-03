using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Beta.Internal.Discovery;
using Beta.Internal.Execution;
using Beta.Internal.Processors;
using Beta.Sdk.Abstractions;
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
    private readonly ITestAssemblyExplorer _testAssemblyExplorer;
    private readonly ITestRunner _testRunner;

    private readonly Test[] _discoveredTests;

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
        _testAssemblyExplorer = serviceProvider.GetRequiredService<ITestAssemblyExplorer>();
        _testRunner = serviceProvider.GetRequiredService<ITestRunner>();
        _discoveredTests = Execute([], () => _testAssemblyExplorer.Explore(testAssembly).ToArray());

        _logger.Debug("Controller initialization complete");
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetaEngineController" /> class.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="testAssembly">The assembly to scan for tests in.</param>
    /// <param name="testAssemblyExplorer">The assembly explorer implementation to use.</param>
    /// <param name="testRunner">The test runner to use.</param>
    /// <remarks>
    ///     This constructor is only meant for unit testing.
    /// </remarks>
    internal BetaEngineController(ILogger logger,
        Assembly testAssembly,
        ITestAssemblyExplorer testAssemblyExplorer,
        ITestRunner testRunner)
    {
        _logger = logger;
        _testAssemblyExplorer = testAssemblyExplorer;
        _testRunner = testRunner;
        _discoveredTests = Execute([], () => _testAssemblyExplorer.Explore(testAssembly).ToArray());
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
    /// Wraps an invocation within a try-catch block to log exceptions.
    /// </summary>
    /// <typeparam name="T">The type to return from the invocation.</typeparam>
    /// <param name="func">The callback to execute.</param>
    /// <param name="caller">The calling method.</param>
    internal void Execute(
        Action func,
        [CallerMemberName] string caller = "")
    {
        try
        {
            _logger.Debug($"Executing controller function [{caller}].");
            func();
        }
        catch (Exception ex)
        {
            _logger.Error("An error occurred while executing a function.", ex);
        }
    }

    /// <summary>
    ///     Queries the test assembly for all tests.
    /// </summary>
    /// <returns>A collection of discovered tests encoded in JSON.</returns>
    public IEnumerable<string> Query() =>
        Execute([], () => from test in _discoveredTests
                          select JsonSerializer.Serialize(new DiscoveredTest
                          {
                              ClassName = test.TestClassName,
                              MethodName = test.Method.Name,
                              Input = test.Input ?? string.Empty,
                              InputIndex = test.InputIndex,
                              TestName = test.FriendlyName
                          }));

    /// <summary>
    ///     Executes a single test.
    /// </summary>
    /// <param name="serializedTest">The serialized request for the test to execute.</param>
    public void Run(string serializedTest) =>
        Execute(() =>
        {
            var test = JsonSerializer.Deserialize<TestInvocationRequest>(serializedTest);

            if (test is null)
            {
                return;
            }

            var matchingTest = _discoveredTests.FirstOrDefault(t =>
                               t.TestClassName == test.ClassName &&
                                              t.Method.Name == test.MethodName &&
                                              t.InputIndex == test.InputIndex);

            var input = test.InputIndex > 0 ? $" with input {test.InputIndex}" : string.Empty;
            var testDescription = $"{test.ClassName}.{test.MethodName}{input}";

            if (matchingTest is null)
            {
                _logger.Error($"Could not find matching test for [{testDescription}].");
                return;
            }

            _logger.Debug($"Executing [{testDescription}]");
            _testRunner.Run(matchingTest);
        });
}
