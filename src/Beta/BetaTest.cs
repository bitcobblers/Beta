using JetBrains.Annotations;

namespace Beta;

[PublicAPI]
public class BetaTest
{
    public delegate void TestConfigurator(TestBuilder builder);

    public delegate void TestConfigurator<in T>(TestBuilder builder, T input);

    public void AddTest(string? name, TestConfigurator configure)
    {
        var builder = new TestBuilder(name);
        configure?.Invoke(builder);
    }

    public void AddTest<T>(string? name, T input, TestConfigurator<T> configure)
    {
        var builder = new TestBuilder(name);
        configure?.Invoke(builder, input);
    }
}