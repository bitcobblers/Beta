using System.Diagnostics.CodeAnalysis;

namespace Beta.Internal;

#pragma warning disable CS8618

/// <summary>
/// Defines a single test that should be executed by the framework.
/// </summary>
[ExcludeFromCodeCoverage]
public record TestInvocationRequest
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
    ///     Gets the index of the input for the test.
    /// </summary>
    public int InputIndex { get; init; }
}