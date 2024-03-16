using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Beta;

public class TestContainer
{
    internal IServiceProvider? ServicesProvider { get; private set; }

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
    protected BetaTest Test<T>(Func<Proof<T>> apply, [CallerMemberName] string testName = "") =>
        new(this, testName, apply);

    protected BetaTest<TInput> Test<TInput, T>(IEnumerable<TInput> scenarios,
                                               Func<TInput, Proof<T>> apply,
                                               [CallerMemberName] string testName = "") =>
        new(this, scenarios, testName, apply);
}
