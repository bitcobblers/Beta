using System.Reflection;
using System.Runtime.CompilerServices;
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
        foreach (var discoveredTest in from method in FindTestMethod(m =>
                                           m.ReturnType.IsAssignableTo(typeof(BetaTest)))
                                       let test =
                                           method.Invoke(this, null) as BetaTest
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
        if (ServicesProvider is not null)
        {
            return;
        }

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
        return new BetaTest(this, testName, apply);
    }

    protected BetaTest<TInput> Test<TInput, T>(IEnumerable<TInput> scenarios, Func<TInput, Proof<T>> apply,
                                               [CallerMemberName] string testName = "")
    {
        return new BetaTest<TInput>(this, scenarios, testName, apply);
    }
}
