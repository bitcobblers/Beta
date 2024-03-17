using Beta.Internal;

namespace Beta.Execution;

public class DefaultTestRunner(ITestSuiteProcessor[] processors, ITestListener listener) : ITestRunner
{
    public async Task Run(IEnumerable<BetaTest> tests, ITestFilter filter, CancellationToken cancellationToken)
    {
        var suites =
            from test in tests
            group test by test.FullyQualifiedTypeName
            into g
            let suite = new
            {
                Instance = g.Key,
                Tests = (from test in g
                         where filter.Filter(test)
                         select test).ToArray()
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
                processors.ForEach(p => p.PreProcess(suite.Instance));

                try
                {
                    await RunTest(test, cancellationToken);
                }
                finally
                {
                    processors.ForEach(p => p.PostProcess(suite.Instance));
                }
            }
        });
    }

    private async Task RunTest(BetaTest test, CancellationToken cancellationToken)
    {
        var result = new BetaTestResult(test.Id);

        listener.OnStart(result.Id, DateTime.UtcNow);

        try
        {
            if (test.SkipReason != null)
            {
                listener.OnFinish(result.Id, DateTime.UtcNow, TestOutcome.Skipped, test.SkipReason);
            }

            if (test.Input == null)
            {
                await foreach (var proofResult in test.Apply(null!).Test(cancellationToken))
                {
                    result.Results.Add(proofResult);
                    listener.OnUpdate(result.Id, proofResult);
                }
            }
            else
            {
                foreach (var input in test.Input)
                {
                    await foreach (var proofResult in test.Apply(input).Test(cancellationToken))
                    {
                        result.Results.Add(proofResult);
                        listener.OnUpdate(result.Id, proofResult);
                    }
                }
            }

            var outcome = result.Results.Count == 0 ? TestOutcome.Inconclusive :
                result.Results.All(r => r.Success) ? TestOutcome.Passed : TestOutcome.Failed;

            listener.OnFinish(result.Id, DateTime.UtcNow, outcome, "");
        }
        catch (Exception ex)
        {
            listener.OnFinish(result.Id, DateTime.UtcNow, TestOutcome.Error, ex.ToString());
        }
    }
}
