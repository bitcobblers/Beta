using Beta.Sdk.Interfaces;

namespace Beta.Internal.Discovery;

/// <summary>
///     Defines an implementation of the test suite activator that uses <see cref="Activator.CreateInstance(Type)" />.
/// </summary>
public class DefaultTestSuiteActivator : ITestSuiteActivator
{
    /// <inheritdoc />
    public TestSuite Create(Type type)
    {
        object? suite;

        try
        {
            suite = Activator.CreateInstance(type);
        }
        catch (Exception ex)
        {
            throw new TestSuiteActivationFailedException("Activator.CreateInstance() threw an exception.", ex);
        }

        if (suite is null)
        {
            throw new TestSuiteActivationFailedException("Activator.CreateInstance() returned null.");
        }

        if (suite is not TestSuite testSuite)
        {
            throw new TestSuiteActivationFailedException(
                "Activator returned a type that is not assignable to TestSuite.");
        }

        return testSuite;
    }
}
