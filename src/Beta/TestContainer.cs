using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Beta;

public class TestContainer
{
    private IServiceProvider? _servicesProvider;

    [PublicAPI]
    public virtual IEnumerable<BetaTest> Discover()
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

        return from method in GetType().GetMethods(flags)
               where method.GetParameters().Length == 0
               where method.ReturnType.IsAssignableTo(typeof(BetaTest))
               select method.Invoke(this, []) as BetaTest;
    }

    public void Prepare()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _servicesProvider = services.BuildServiceProvider();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }

    // ---

    [PublicAPI]
    protected StepResult<T> Require<T>() where T : notnull => new(() => _servicesProvider.GetRequiredService<T>());

    [PublicAPI]
    protected StepResult<object> Require(Type type) => new(() => _servicesProvider.GetRequiredService(type));

    [PublicAPI]
    protected StepResult<T> Gather<T>(T value) => new(() => value);

    [PublicAPI]
    protected StepResult<T> Gather<T>(Func<T> handler) => new(handler);

    [PublicAPI]
    protected Axiom<T> Apply<T>(Func<T> handler) => new(handler());

    [PublicAPI]
    protected Axiom<T> Apply<T>(Func<Task<T>> handler) => new(handler());

    // ---

    [PublicAPI]
    protected BetaTest Test<T>(Axiom<T> axiom)
    {
        return new BetaTest<T>(this, axiom);
    }

    [PublicAPI]
    protected IEnumerable<BetaTest> Test<TInput, T>(IScenarioSource<TInput> scenarios, Func<TInput, Axiom<T>> apply)
    {
        return from scenario in scenarios
               select Test(apply(scenario));
    }

    protected IEnumerable<BetaTest> Test<TInput, T>(IEnumerable<TInput> scenarios, Func<TInput, Axiom<T>> apply)
    {
        return from scenario in scenarios
               select Test(apply(scenario));
    }
}
