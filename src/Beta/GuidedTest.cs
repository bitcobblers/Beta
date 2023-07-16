using JetBrains.Annotations;

namespace Beta;

public class GuidedTest : BaseTest
{
    [PublicAPI]
    public void Test(string? name, TestConfigurator<GuidedTestBuilder> test)
    {
        AddTest(name, test);
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
