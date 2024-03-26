using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Beta.TestAdapter.Models;

/// <summary>
///     Defines a single discovered test case.
/// </summary>
[ExcludeFromCodeCoverage]
public record DiscoveredTest
{
    /// <summary>
    ///     Gets the class name of the test.
    /// </summary>
    public string ClassName { get; init; }

    /// <summary>
    ///     Gets the method name of the test.
    /// </summary>
    public string MethodName { get; init; }

    /// <summary>
    ///     Gets the input to apply to the test case.
    /// </summary>
    public string Input { get; init; }

    /// <summary>
    ///     Gets the friendly name of the test.
    /// </summary>
    public string TestName { get; init; }
}
