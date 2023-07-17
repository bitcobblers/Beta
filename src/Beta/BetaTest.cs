using JetBrains.Annotations;

namespace Beta;

public class BetaTest : BaseTest
{
    [PublicAPI]
    public void BasicTest(string? name, TestConfigurator<BasicTestBuilder> test)
    {
        AddTest(name, new TestConfigurator<BasicTestBuilder>(b =>
        {
            (b as BaseTestBuilder).UpdateHandler(() => test(b));
        }));
    }

    [PublicAPI]
    public void BasicTest<TInput>(string? name, IEnumerable<TInput> source, TestConfigurator<BasicTestBuilder, TInput> test)
    {
        foreach (var input in source)
        {
            AddTest(name, input, new TestConfigurator<BasicTestBuilder, TInput>((b, i) =>
            {
                (b as BaseTestBuilder).UpdateHandler(() => test(b, i));
            }));
        }
    }

    [PublicAPI]
    public void GuidedTest(string? name, TestConfigurator<GuidedTestBuilder> test)
    {
        AddTest(name, test);
    }

    [PublicAPI]
    public void GuidedTest<TInput>(string? name, IEnumerable<TInput> source, TestConfigurator<GuidedTestBuilder, TInput> test)
    {
        foreach (var input in source)
        {
            AddTest(name, input, test);
        }
    }
}
