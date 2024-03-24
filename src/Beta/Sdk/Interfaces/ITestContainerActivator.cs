namespace Beta.Sdk.Interfaces;

/// <summary>
///     Defines a system used to create instances of test containers.
/// </summary>
public interface ITestContainerActivator
{
    /// <summary>
    ///     Creates a new instance of a test suite.
    /// </summary>
    /// <param name="type">The suite type to create.</param>
    /// <returns>A new instance of the test suite.</returns>
    object Create(Type type);
}