using System.Runtime.CompilerServices;
using Beta.Internal.Processors;
using Beta.Sdk.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Beta;

/// <summary>
///     Defines the base suite class used for defining tests.
/// </summary>
[PublicAPI]
public class TestSuite
{
    private readonly List<ITestSuiteProcessor> _postprocessors = [];
    private readonly List<ITestSuiteProcessor> _preprocessors = [];

    /// <summary>
    ///     Gets the collection of pre-processors to apply.
    /// </summary>
    public IEnumerable<ITestSuiteProcessor> PreProcessors => _preprocessors;

    /// <summary>
    ///     Gets the collection of post-processors to apply.
    /// </summary>
    public IEnumerable<ITestSuiteProcessor> PostProcessors => _postprocessors;

    /// <summary>
    ///     Registers a pre-processor to apply before test execution.
    /// </summary>
    /// <param name="processor">The processor to apply.</param>
    protected void AddPreProcessor(ITestSuiteProcessor processor) =>
        _preprocessors.Add(processor);

    /// <summary>
    ///     Registers a post-processor to apply after to test execution.
    /// </summary>
    /// <param name="processor">The processor to apply.</param>
    protected void AddPostProcessor(ITestSuiteProcessor processor) =>
        _postprocessors.Add(processor);

    /// <summary>
    ///     Allows optional setup code to execute just prior to a test.
    /// </summary>
    [PublicAPI]
    protected virtual void Setup()
    {
    }

    /// <summary>
    ///     Allows optional teardown code to execute just after a test.
    /// </summary>
    [PublicAPI]
    protected virtual void Teardown()
    {
    }

    /// <summary>
    ///     Create a new gather step.
    /// </summary>
    /// <param name="value">The value to capture for the step.</param>
    /// <typeparam name="T">The type of value to capture.</typeparam>
    /// <returns>A new gather step.</returns>
    protected static Step<T> Gather<T>(T value) =>
        new(() => value);

    /// <summary>
    ///     Creates a new gather step.
    /// </summary>
    /// <param name="handler">The handler used to capture the value for the step.</param>
    /// <typeparam name="T">The type of value to capture.</typeparam>
    /// <returns>A new gather step.</returns>
    protected static Step<T> Gather<T>(Func<T> handler) =>
        new(handler);

    /// <summary>
    ///     Creates a new proof.
    /// </summary>
    /// <param name="handler">The handler used to capture the value for the proof.</param>
    /// <typeparam name="T">The type of value to capture.</typeparam>
    /// <returns>A new <see cref="Proof{T}" /> to assert on.</returns>
    protected static Proof<T> Apply<T>(Func<T> handler) =>
        new(handler());

    /// <summary>
    ///     Creates a new proof.
    /// </summary>
    /// <param name="handler">The async handler used to capture the value for the proof.</param>
    /// <typeparam name="T">The type of value to capture.</typeparam>
    /// <returns>A new <see cref="Proof{T}" /> to assert on.</returns>
    protected static Proof<T> Apply<T>(Func<Task<T>> handler) =>
        new(handler());

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

    /// <summary>
    ///     Defines an implementation of the <see cref="TestSuite" /> that supports DI containers.
    /// </summary>
    public class DI : TestSuite
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TestSuite.DI" /> class.
        /// </summary>
        public DI()
        {
            AddPreProcessor(new InitializeContainerProcessor());
        }

        /// <summary>
        ///     The underlying service provider.
        /// </summary>
        internal IServiceProvider? ServicesProvider { get; private set; }

        /// <summary>
        ///     Initializes the suite.
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

        /// <summary>
        ///     Creates a new step that resolves a service from the DI container.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>A new step.</returns>
        protected Step<T> Require<T>() where T : notnull =>
            new(() => ServicesProvider!.GetRequiredService<T>());

        /// <summary>
        ///     Creates a new step that resolves a service from the DI container.
        /// </summary>
        /// <param name="type">The type of service to resolve.</param>
        /// <returns>A new step.</returns>
        protected Step<object> Require(Type type) =>
            new(() => ServicesProvider!.GetRequiredService(type));
    }
}
