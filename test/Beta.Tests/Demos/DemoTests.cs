using Beta.Internal.Discovery;
using Beta.Internal.Execution;
using Beta.Sdk.Interfaces;
using Xunit.Abstractions;

namespace Beta.Tests.Demos;

public class DemoTests(ITestOutputHelper output)
{
    [Fact]
    public Task RunTestsFromType()
    {
        var logger = new XUnitLogger(output);

        var activator = new DefaultTestSuiteActivator();
        var testCaseDiscoverer = new DefaultTestCaseDiscoverer(activator);
        var discoverer = new DefaultTestDiscoverer(testCaseDiscoverer);
        var aggregator = new DefaultTestSuiteAggregator([discoverer]);

        var runner = new DefaultTestRunner(logger, A.Fake<ITestListener>());

        return runner.Run(
            aggregator.Aggregate([typeof(CalculatorDemo)]),
            A.Fake<ITestFilter>(),
            CancellationToken.None);
    }
}
