using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        foreach (var discoveredTest in from method in FindTestMethod(m =>
                                           m.ReturnType.IsAssignableTo(typeof(BetaTest)))
                                       let test =
                                           method.Invoke(this, Array.Empty<object>()) as BetaTest
                                       where test is not null
                                       select test with
                                       {
                                           Method = method
                                       })
        {
            yield return discoveredTest;
        }

        foreach (var discoveredTest in from method in FindTestMethod(m =>
                                           m.ReturnType.IsAssignableTo(typeof(IEnumerable<BetaTest>)))
                                       let tests =
                                           method.Invoke(this, Array.Empty<object>()) as IEnumerable<BetaTest>
                                       where tests is not null
                                       from test in tests
                                       where test is not null
                                       select test with
                                       {
                                           Method = method
                                       })
        {
            yield return discoveredTest;
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

    // ---

    [PublicAPI]
    protected BetaTest Test<T>(Func<Proof<T>> apply, [CallerMemberName] string testName = "")
    {
        return new BetaTest(this, null, testName, apply);
    }

    protected IEnumerable<BetaTest> Test<TInput, T>(IEnumerable<TInput> scenarios, Func<TInput, Proof<T>> apply, [CallerMemberName] string testName = "")
    {
        return from scenario in scenarios
               select new BetaTest(this, scenario, testName, () => apply(scenario));
    }
}
