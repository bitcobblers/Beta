using Beta.Sdk.Interfaces;

namespace Beta.Internal.Discovery;

/// <summary>
///     Defines an implementation of the test suite activator that uses <see cref="Activator.CreateInstance(Type)" />.
/// </summary>
public class DefaultTestSuiteActivator : ITestSuiteActivator
{
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultTestSuiteActivator" /> class.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    // ReSharper disable once ConvertToPrimaryConstructor
    public DefaultTestSuiteActivator(ILogger logger) =>
        _logger = logger.CreateScope("activator");

    /// <inheritdoc />
    public TestSuite? Create(Type type)
    {
        object? suite;

        try
        {
            _logger.Debug($"Suite activator creating an instance of type [{type.FullName}]");
            suite = Activator.CreateInstance(type);
        }
        catch (Exception ex)
        {
            // The activator will wrap the exception with its own.
            if (ex.InnerException is not null)
            {
                ex = ex.InnerException;
            }

            _logger.Error("Suite activator threw an exception.", ex);
            return null;
        }

        switch (suite)
        {
            case TestSuite testSuite:
                return testSuite;
            case null:
                _logger.Error("Suite activator returned null");
                return null;
            default:
                _logger.Error($"Suite activator returned a type that is not assignable to [{nameof(TestSuite)}].");
                return null;
        }
    }
}
