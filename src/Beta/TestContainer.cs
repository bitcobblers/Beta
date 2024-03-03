using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Beta;

public class TestContainer
{
    internal IServiceProvider? ServicesProvider { get; private set; }

    private IEnumerable<MethodInfo> FindTestMethod(Func<MethodInfo, bool> predicate)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

        return from method in GetType().GetMethods(flags)
               where method.GetParameters().Length == 0
               where method.GetCustomAttribute<IgnoreDiscoveryAttribute>() is null
               where predicate(method)
               select method;
    }

    [IgnoreDiscovery]
    public IEnumerable<BetaTest> Discover()
    {
        foreach (var test in from method in FindTestMethod(method => method.ReturnType.IsAssignableTo(typeof(BetaTest)))
                             let test = method.Invoke(this, []) as BetaTest
                             where test is not null
                             select test)
        {
            yield return test;
        }

        foreach (var test in from method in FindTestMethod(method =>
                                 method.ReturnType.IsAssignableTo(typeof(IEnumerable<BetaTest>)))
                             let tests = method.Invoke(this, []) as IEnumerable<BetaTest>
                             where tests is not null
                             from test in tests
                             where test is not null
                             select test)
        {
            yield return test;
        }
    }

    public void Prepare()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServicesProvider = services.BuildServiceProvider();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }

    // ---

    [PublicAPI]
    protected Step<T> Require<T>() where T : notnull
    {
        return new Step<T>(() => ServicesProvider!.GetRequiredService<T>());
    }

    [PublicAPI]
    protected Step<object> Require(Type type)
    {
        return new Step<object>(() => ServicesProvider!.GetRequiredService(type));
    }

    [PublicAPI]
    protected Step<T> Gather<T>(T value)
    {
        return new Step<T>(() => value);
    }

    [PublicAPI]
    protected Step<T> Gather<T>(Func<T> handler)
    {
        return new Step<T>(handler);
    }

    [PublicAPI]
    protected Proof<T> Apply<T>(Func<T> handler)
    {
        return new Proof<T>(handler());
    }

    [PublicAPI]
    protected Proof<T> Apply<T>(Func<Task<T>> handler)
    {
        return new Proof<T>(handler());
    }

    // ---

    [PublicAPI]
    protected BetaTest Test<T>(Proof<T> proof)
    {
        return new BetaTest<T>(this, proof);
    }

    [PublicAPI]
    protected IEnumerable<BetaTest> Test<TInput, T>(IScenarioSource<TInput> scenarios, Func<TInput, Proof<T>> apply)
    {
        return from scenario in scenarios
               select Test(apply(scenario));
    }

    protected IEnumerable<BetaTest> Test<TInput, T>(IEnumerable<TInput> scenarios, Func<TInput, Proof<T>> apply)
    {
        return from scenario in scenarios
               select Test(apply(scenario));
    }
}