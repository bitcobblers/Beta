using JetBrains.Annotations;

namespace Beta;

[PublicAPI]
public class BetaTest
{
    private readonly List<TestDefinition> _tests = new();

    public delegate void TestConfigurator(TestBuilder builder);

    public delegate void TestConfigurator<in T>(TestBuilder builder, T input);

    public void AddTest(string? name, TestConfigurator configure)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var builder = new TestBuilder(name);
        configure?.Invoke(builder);

        _tests.Add(new TestDefinition(builder.Name, null, builder.Handler));
    }

    public void AddTest<T>(string? name, T input, TestConfigurator<T> configure)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var builder = new TestBuilder(name);
        configure?.Invoke(builder, input);

        _tests.Add(new TestDefinition(builder.Name, input, builder.Handler));
    }

    public void Execute()
    {
        foreach (var test in _tests)
        {
            test.Handler.DynamicInvoke();
        }
    }
}

public record TestDefinition(string Name, object? Input, Delegate Handler);