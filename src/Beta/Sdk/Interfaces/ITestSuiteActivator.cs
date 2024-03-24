namespace Beta.Sdk.Interfaces;

/// <summary>
///     Defines a system used to create instances of test containers.
/// </summary>
public interface ITestSuiteActivator
{
    /// <summary>
    ///     Creates a new instance of a test suite.
    /// </summary>
    /// <param name="type">The suite type to create.</param>
    /// <returns>A new instance of the test suite.</returns>
    TestSuite Create(Type type);
}
