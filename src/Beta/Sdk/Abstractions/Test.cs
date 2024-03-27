using System.Reflection;

namespace Beta.Sdk.Abstractions;

/// <summary>
///     Defines a single test that can be executed by the runner.
/// </summary>
public record Test(TestSuite Instance, MethodInfo Method, Func<Proof> Apply)
{
    /// <summary>
    ///     Gets the id of the test.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the friendly name of the test.
    /// </summary>
    public string FriendlyName { get; init; } = string.Empty;

    /// <summary>
    ///     gets the fully qualified name of the test class.
    /// </summary>
    public string TestClassName { get; } = Method.DeclaringType!.FullName!;

    /// <summary>
    ///     Gets the serialized input for the test.
    /// </summary>
    public string? Input { get; init; }
}
