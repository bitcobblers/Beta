using Beta.Sdk.Interfaces;

namespace Beta.Internal.Discovery;

/// <summary>
///     Defines an implementation of the test suite activator that uses <see cref="Activator.CreateInstance(Type)" />.
/// </summary>
public class DefaultTestSuiteActivator(ILogger logger) : ITestSuiteActivator
{
    /// <inheritdoc />
    public TestSuite? Create(Type type)
    {
        object? suite;

        try
        {
            logger.Debug($"Suite activator creating an instance of type {type.FullName}");
            suite = Activator.CreateInstance(type);
        }
        catch (Exception ex)
        {
            // The activator will wrap the exception with its own.
            if (ex.InnerException is not null)
            {
                ex = ex.InnerException;
            }

            logger.Error("Suite activator threw an exception.", ex);
            return null;
        }

        switch (suite)
        {
            case TestSuite testSuite:
                return testSuite;
            case null:
                logger.Error("Suite activator returned null");
                return null;
            default:
                logger.Error($"Suite activator returned a type that is not assignable to {nameof(TestSuite)}.");
                return null;
        }
    }
}
