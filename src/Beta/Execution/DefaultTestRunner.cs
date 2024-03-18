using Beta.Discovery;
using Beta.Internal;

namespace Beta.Execution;

/// <summary>
///     Defines the default test runner.
/// </summary>
/// <param name="logger">The internal logger to use.</param>
/// <param name="processors">The suite processors to use.</param>
/// <param name="listener">The test listener to use.</param>
public class DefaultTestRunner(ILogger logger, ITestSuiteProcessor[] processors, ITestListener listener) : ITestRunner
{
    /// <inheritdoc />
    public async Task Run(IEnumerable<Test> tests, ITestFilter filter, CancellationToken cancellationToken)
    {
        var suites =
            from test in tests
            group test by test.TestClassName
            into g
            let suite = new
            {
                Instance = g.Key,
                Tests = g.ToArray()
            }
            where suite.Tests.Length > 0
            select suite;

        await Parallel.ForEachAsync(suites, cancellationToken, async (suite, token) =>
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            foreach (var test in suite.Tests)
            {
                processors.ForEach(p => p.PreProcess(test.Instance));

                try
                {
                    //await RunTest(test, cancellationToken);
                    await foreach (var proofResult in test.Apply().Test(cancellationToken))
                    {
                        // result.Results.Add(proofResult);
                        // listener.OnUpdate(result.Id, proofResult);
                        logger.Log(proofResult.ToString());
                    }
                }
                finally
                {
                    processors.ForEach(p => p.PostProcess(test.Instance));
                }
            }
        });
    }
}
