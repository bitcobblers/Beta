using System.Reflection;

namespace Beta.Discovery;

/// <summary>
///     Defines a single test that can be executed by the runner.
/// </summary>
public class Test
{
    /// <summary>
    ///     Gets the id of the test.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     gets the fully qualified name of the test class.
    /// </summary>
    public string TestClassName { get; init; }

    /// <summary>
    ///     Gets the instance of the test class.
    /// </summary>
    public object Instance { get; init; }

    /// <summary>
    ///     Gets the method that defined the test.
    /// </summary>
    public MethodInfo Method { get; init; }

    /// <summary>
    ///     Gets the serialized input for the test.
    /// </summary>
    public string? Input { get; init; }

    /// <summary>
    ///     Gets the apply method to execute the test.
    /// </summary>
    public Func<Proof> Apply { get; init; }
}
