using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Beta;

/// <summary>
///     Defines a container for defining tests.
/// </summary>
public class TestContainer
{
    internal IServiceProvider? ServicesProvider { get; private set; }

    /// <summary>
    ///     Initializes the container.
    /// </summary>
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

    /// <summary>
    ///     Called before test execution to allow for service configuration.
    /// </summary>
    /// <param name="services">The services to configure.</param>
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

    /// <summary>
    ///     Defines a new singular test that does not use any input.
    /// </summary>
    /// <param name="apply">The apply method of the test.</param>
    /// <param name="testName">The optional name of the test.</param>
    /// <typeparam name="T">The type of <see cref="Proof{T}" /> created by the test.</typeparam>
    /// <returns>A singular test.</returns>
    protected BetaTest Test<T>(Func<Proof<T>> apply, [CallerMemberName] string testName = "") =>
        new(this, testName, apply);

    /// <summary>
    ///     Defines an aggregate test that uses input from a source.
    /// </summary>
    /// <param name="scenarios">The input scenarios for the test.</param>
    /// <param name="apply">The apply method of the test.</param>
    /// <param name="testName">The optional name of the test.</param>
    /// <typeparam name="TInput">The type of input pass into the apply method.</typeparam>
    /// <typeparam name="T">The type of <see cref="Proof{T}" /> created by the test.</typeparam>
    /// <returns>An aggregate test.</returns>
    protected BetaTest<TInput> Test<TInput, T>(IEnumerable<TInput> scenarios,
                                               Func<TInput, Proof<T>> apply,
                                               [CallerMemberName] string testName = "") =>
        new(this, scenarios, testName, apply);
}
