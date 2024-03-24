using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Beta.Api.Contracts;

/// <summary>
///     Defines a single discovered test.
/// </summary>
[ExcludeFromCodeCoverage]
public class DiscoveredTest
{
    /// <summary>
    ///     Gets the fully qualified name of the class that defined the test.
    /// </summary>
    public string ClassName { get; init; }

    /// <summary>
    ///     Gets the name of the test method.
    /// </summary>
    public string MethodName { get; init; }

    /// <summary>
    ///     Gets the stringified input for the test.
    /// </summary>
    public string Input { get; init; }

    /// <summary>
    ///     Gets the friendly name of the test.
    /// </summary>
    public string TestName { get; init; }
}
