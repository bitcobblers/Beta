using JetBrains.Annotations;

namespace Beta;

[PublicAPI]
public class BaseTest
{
    private readonly Dictionary<string, List<TestDefinition>> _tests = new();

    public delegate void TestConfigurator<in TBuilder>(TBuilder builder) where TBuilder : BaseTestBuilder;

    public delegate void TestConfigurator<in TBuilder, in TInput>(TBuilder builder, TInput input) where TBuilder : BaseTestBuilder;

    protected void AddTest<TBuilder>(string? name, TestConfigurator<TBuilder> configure)
        where TBuilder : BaseTestBuilder, new()
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var builder = new TBuilder();
        configure?.Invoke(builder);

        RegisterTest(new TestDefinition(name, null, builder.Handler));
    }

    protected void AddTest<TBuilder, TInput>(string? name, TInput input, TestConfigurator<TBuilder, TInput> configure)
        where TBuilder : BaseTestBuilder, new()
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var builder = new TBuilder();
        configure?.Invoke(builder, input);

        RegisterTest(new TestDefinition(name, input, builder.Handler));
    }

    public void Execute()
    {
        foreach (var (_, tests) in _tests)
        {
            foreach (var test in tests)
            {
                test.Handler.DynamicInvoke();
            }
        }
    }

    private void RegisterTest(TestDefinition test)
    {
        if (_tests.TryGetValue(test.Name, out var testList))
        {
            testList.Add(test);
        }
        else
        {
            _tests.Add(test.Name, new List<TestDefinition> { test });
        }
    }
}