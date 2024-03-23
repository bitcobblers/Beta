using System.Diagnostics.CodeAnalysis;

namespace Beta.TestAdapter.Exceptions;

/// <summary>
///     Defines an exception that is thrown when the Beta controller instantiation fails.
/// </summary>
/// <param name="message">The reason the controller failed to instantiate.</param>
[ExcludeFromCodeCoverage]
public class BetaControllerInstantiationFailedException(string message)
    : Exception(message);
