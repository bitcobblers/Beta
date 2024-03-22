namespace Beta.Discovery;

/// <summary>
///     Defines a system used to create instances of test containers.
/// </summary>
public interface ITestContainerActivator
{
    /// <summary>
    ///     Creates a new instance of a test container.
    /// </summary>
    /// <param name="type">The container type to create.</param>
    /// <returns>A new instance of the test container.</returns>
    object Create(Type type);
}