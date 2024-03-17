using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Beta;

public class TestContainer
{
    internal IServiceProvider? ServicesProvider { get; private set; }

    public void Initialize()
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

    protected Step<T> Require<T>() where T : notnull =>
        new(() => ServicesProvider!.GetRequiredService<T>());

    protected Step<object> Require(Type type) =>
        new(() => ServicesProvider!.GetRequiredService(type));

    protected static Step<T> Gather<T>(T value) =>
        new(() => value);

    protected static Step<T> Gather<T>(Func<T> handler) =>
        new(handler);

    protected static Proof<T> Apply<T>(Func<T> handler) =>
        new(handler());

    protected static Proof<T> Apply<T>(Func<Task<T>> handler) =>
        new(handler());

    // ---

    protected BetaTest Test<T>(Func<Proof<T>> apply, [CallerMemberName] string testName = "") =>
        new(this, testName, apply);

    protected BetaTest<TInput> Test<TInput, T>(IEnumerable<TInput> scenarios,
                                               Func<TInput, Proof<T>> apply,
                                               [CallerMemberName] string testName = "") =>
        new(this, scenarios, testName, apply);
}
