using Beta.Internal.Discovery;
using Beta.Internal.Execution;
using Beta.Internal.Processors;
using Beta.Sdk.Interfaces;
using Xunit.Abstractions;

namespace Beta.Tests.Demos;

public class XUnitLogger(ITestOutputHelper output) : ILogger
{
    /// <inheritdoc />
    public void Log(string message)
    {
        output.WriteLine(message);
    }
}

public class DemoTests(ITestOutputHelper output)
{
    [Fact]
    public Task RunTestsFromType()
    {
        var logger = new XUnitLogger(output);

        var activator = new DefaultTestContainerActivator();
        var testCaseDiscoverer = new DefaultTestCaseDiscoverer(activator);
        var discoverer = new DefaultTestDiscoverer(testCaseDiscoverer);
        var aggregator = new DefaultTestSuiteAggregator([discoverer]);
        ITestSuiteProcessor[] processors = [new InitializeContainerProcessor()];

        var runner = new DefaultTestRunner(logger, processors, A.Fake<ITestListener>());

        return runner.Run(
            aggregator.Aggregate([typeof(CalculatorDemo)]),
            A.Fake<ITestFilter>(),
            CancellationToken.None);
    }
}
