using Beta.Sdk.Interfaces;

namespace Beta.Internal.Discovery;

/// <summary>
///     Defines an implementation of the test suite activator that uses <see cref="Activator.CreateInstance(Type)" />.
/// </summary>
public class DefaultTestContainerActivator : ITestContainerActivator
{
    /// <inheritdoc />
    public object Create(Type type) =>
        Activator.CreateInstance(type) ??
        throw new TestContainerActivationFailedException("Activator.CreateInstance() returned null.");
}