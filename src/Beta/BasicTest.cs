using JetBrains.Annotations;

namespace Beta;

public class BasicTest : BetaTest
{
    [PublicAPI]
    public void Test(string? name, Action test)
    {
        AddTest<BasicTestBuilder>(name, configure =>
        {
            configure.SetHandler(_ => new BasicStepBuilder(test));
        });
    }

    [PublicAPI]
    public void Test<TInput>(string? name, IEnumerable<TInput> source, Action<TInput> test)
    {
        foreach (var input in source)
        {
            AddTest<BasicTestBuilder, TInput>(name, input, (configure, _) =>
            {
                configure.SetHandler(_ => new BasicStepBuilder(() => test(input)));
            });
        }
    }
}