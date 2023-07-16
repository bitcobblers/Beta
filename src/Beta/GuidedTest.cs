using JetBrains.Annotations;

namespace Beta;

public class GuidedTest : BaseTest
{
    [PublicAPI]
    public void Test(string? name)
    {
        AddTest<GuidedTestBuilder>(name, configure =>
        {

        });
    }

    [PublicAPI]
    public void Test<TInput>(string? name, IEnumerable<TInput> source, TestConfigurator<GuidedTestBuilder, TInput> test)
    {
        foreach (var input in source)
        {
            AddTest(name, input, test);
        }
    }
}