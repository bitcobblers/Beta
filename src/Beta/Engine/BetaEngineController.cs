using System.Reflection;
using System.Xml.Linq;
using Beta.Discovery;
using Beta.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Beta.Engine;

public class BetaEngineController
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Assembly _testAssembly;

    private readonly ITestAssemblyExplorer _testAssemblyExplorer;

    public BetaEngineController(Assembly testAssembly)
    {
        var serviceCollection = new ServiceCollection();

        // TODO: Replace static discovery with dynamic discovery.
        serviceCollection.AddSingleton<ITestAssemblyExplorer, DefaultTestAssemblyExplorer>();
        serviceCollection.AddSingleton<ITestCaseDiscoverer, DefaultTestCaseDiscoverer>();
        serviceCollection.AddSingleton<ITestContainerActivator, DefaultTestContainerActivator>();
        serviceCollection.AddSingleton<ITestDiscoverer, DefaultTestDiscoverer>();
        serviceCollection.AddSingleton<ITestSuiteAggregator, DefaultTestSuiteAggregator>();
        serviceCollection.AddSingleton<ITestRunner, DefaultTestRunner>();
        serviceCollection.AddSingleton<ITestSuiteProcessor, TestContainerProcessor>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _testAssembly = testAssembly;
        _testAssemblyExplorer = _serviceProvider.GetRequiredService<ITestAssemblyExplorer>();
    }

    public IEnumerable<XElement> Query()
    {
        return from test in _testAssemblyExplorer.Explore(_testAssembly)
               select new XElement("test",
                   new XAttribute("id", test.Id),
                   new XElement("className", test.TestClassName),
                   new XElement("methodName", test.Method.Name),
                   new XElement("input", test.Input ?? string.Empty));
    }

    public void Run()
    {
    }

    public void Stop()
    {
    }
}
