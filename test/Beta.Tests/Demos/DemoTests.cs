using Beta.Discovery;
using Xunit.Abstractions;

namespace Beta.Tests.Demos;

public class DemoTests(ITestOutputHelper output)
{
    [Fact]
    public async Task RunTestsFromType()
    {
        var discoverer = new DefaultTestDiscoverer();
        var aggregator = new DefaultTestSuiteAggregator([discoverer]);
        var processors = new[] { new TestContainerProcessor() };

        var suites =
            from test in aggregator.Aggregate([typeof(CalculatorDemo)])
            group test by test.Container
            into g
            select new TestSuite
            {
                Instance = g.Key,
                Tests = g.ToArray()
            };

        foreach (var suite in suites)
        {
            foreach (var processor in processors)
            {
                processor.PreProcess(suite.Instance);
            }

            foreach (var test in suite.Tests)
            {
                if (test.Input == null)
                {
                    await foreach (var result in test.Apply(null!).Test())
                    {
                        output.WriteLine(result.ToString());
                    }
                }
                else
                {
                    foreach (var input in test.Input)
                    {
                        await foreach (var result in test.Apply(input).Test())
                        {
                            output.WriteLine(result.ToString());
                        }
                    }
                }
            }

            foreach (var processor in processors)
            {
                processor.PostProcess(suite.Instance);
            }
        }

        // var calcDemo = new CalculatorDemo();
        // calcDemo.Prepare();
        //
        // var tests = calcDemo.AddTestMany();
        //
        //
        //
        // foreach (var test in tests)
        // {
        //     await test.Prove();
        // }
    }
}
