using System.Reflection;
using Beta.Api;
using Beta.Api.Contracts;
using Beta.Internal.Discovery;
using Beta.Internal.Execution;
using Beta.Internal.Processors;
using Beta.Sdk.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Beta.Internal;

[PublicAPI]
public class BetaEngineController : IEngineController
{
    private readonly Assembly _testAssembly;

    private readonly ITestAssemblyExplorer _testAssemblyExplorer;

    public BetaEngineController(Assembly testAssembly)
    {
        var serviceCollection = new ServiceCollection();

        // TODO: Replace static discovery with dynamic discovery.
        serviceCollection.AddSingleton<ITestAssemblyExplorer, DefaultTestAssemblyExplorer>();
        serviceCollection.AddSingleton<ITestCaseDiscoverer, DefaultTestCaseDiscoverer>();
        serviceCollection.AddSingleton<ITestSuiteActivator, DefaultTestSuiteActivator>();
        serviceCollection.AddSingleton<ITestDiscoverer, DefaultTestDiscoverer>();
        serviceCollection.AddSingleton<ITestSuiteAggregator, DefaultTestSuiteAggregator>();
        serviceCollection.AddSingleton<ITestRunner, DefaultTestRunner>();
        serviceCollection.AddSingleton<ITestSuiteProcessor, InitializeContainerProcessor>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        _testAssembly = testAssembly;
        _testAssemblyExplorer = serviceProvider.GetRequiredService<ITestAssemblyExplorer>();
    }

    public IEnumerable<DiscoveredTest> Query() =>
        from test in _testAssemblyExplorer.Explore(_testAssembly)
        select new DiscoveredTest
        {
            ClassName = test.TestClassName,
            MethodName = test.Method.Name,
            Input = test.Input ?? string.Empty,
            TestName = string.Empty
        };
}
