using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Beta;

public abstract class BaseTest
{
    private readonly Dictionary<string, List<TestDefinition>> _tests = new();
    private ServiceProvider? _serviceProvider;

    public delegate void TestConfigurator<in TBuilder>(TBuilder builder) where TBuilder : BaseTestBuilder;
    public delegate void TestConfigurator<in TBuilder, in TInput>(TBuilder builder, TInput input) where TBuilder : BaseTestBuilder;

    [PublicAPI]
    public void Discover()
    {
        InitializeContainer(runUserConfiguration: false);
        DefineTests();
    }

    [PublicAPI]
    public void Execute()
    {
        InitializeContainer(runUserConfiguration: true);
        DefineTests();

        foreach (var (_, tests) in _tests)
        {
            foreach (var test in tests)
            {
                test.Handler.DynamicInvoke();
            }
        }
    }

    /// <summary>
    /// Overridden by test classes to define their tests.
    /// </summary>
    protected virtual void DefineTests()
    {
    }

    /// <summary>
    /// Optionally overridden by test classes to define any custom injections.
    /// </summary>
    /// <param name="services">The service catalog to register types with.</param>
    protected virtual void ConfigureContainer(IServiceCollection services)
    {
    }

    private void InitializeContainer(bool runUserConfiguration)
    {
        if (_serviceProvider != null)
        {
            return;
        }

        var services = new ServiceCollection();

        services.AddTransient<BasicTestBuilder>();
        services.AddTransient<GuidedTestBuilder>();

        if (runUserConfiguration)
        {
            ConfigureContainer(services);
        }

        _serviceProvider = services.BuildServiceProvider();
    }

    protected void AddTest<TBuilder>(string? name, TestConfigurator<TBuilder> configure)
        where TBuilder : BaseTestBuilder
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var builder = _serviceProvider!.GetRequiredService<TBuilder>();
        configure?.Invoke(builder);

        if (builder.IsValid(out var errors) == false)
        {
            throw new IncorrectlyConfiguredTestException(name, errors.ToArray());
        }

        RegisterTest(new TestDefinition(name, null, builder.Handler));
    }

    protected void AddTest<TBuilder, TInput>(string? name, TInput input, TestConfigurator<TBuilder, TInput> configure)
        where TBuilder : BaseTestBuilder
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var builder = _serviceProvider!.GetRequiredService<TBuilder>();
        configure?.Invoke(builder, input);

        if (builder.IsValid(out var errors) == false)
        {
            throw new IncorrectlyConfiguredTestException(name, errors.ToArray());
        }

        RegisterTest(new TestDefinition(name, input, builder.Handler));
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