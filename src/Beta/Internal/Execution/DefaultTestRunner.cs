using Beta.Sdk.Abstractions;
using Beta.Sdk.Interfaces;

namespace Beta.Internal.Execution;

/// <summary>
///     Defines the default test runner.
/// </summary>
/// <param name="logger">The internal logger to use.</param>
/// <param name="listener">The test listener to use.</param>
public class DefaultTestRunner(ILogger logger, ITestListener listener)
    : ITestRunner
{
    /// <inheritdoc />
    public Task Run(IEnumerable<Test> tests, ITestFilter filter, CancellationToken cancellationToken)
    {
        var suites =
            from test in tests
            group test by test.TestClassName
            into g
            let suite = new
            {
                Instance = g.Key,
                Tests = (from t in g
                         where filter.Filter(t)
                         select t).ToArray()
            }
            where suite.Tests.Length > 0
            select suite;

        return Parallel.ForEachAsync(suites, cancellationToken, async (suite, token) =>
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            foreach (var test in suite.Tests)
            {
                test.Instance.PreProcessors.ForEach(p => p.Process(test.Instance));
                var result = new BetaTestResult(test.Id);
                var outcome = TestOutcome.Passed;

                try
                {
                    await foreach (var proofResult in test.Apply().Test(cancellationToken))
                    {
                        result.Results.Add(proofResult);
                        listener.OnUpdate(result.Id, proofResult);
                        logger.Debug(proofResult.ToString());

                        if (proofResult.Success == false)
                        {
                            outcome = TestOutcome.Failed;
                        }
                    }

                    listener.OnFinish(test.Id, DateTime.UtcNow, outcome, string.Empty);
                }
                finally
                {
                    test.Instance.PostProcessors.ForEach(p => p.Process(test.Instance));
                }
            }
        });
    }
}
