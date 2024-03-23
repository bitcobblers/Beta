using System.Diagnostics.CodeAnalysis;

namespace Beta.TestAdapter.Exceptions;

/// <summary>
///     Defines an exception that is thrown when the Beta controller is not found.
/// </summary>
/// <param name="message">A message describing why the controller could not be found.</param>
[ExcludeFromCodeCoverage]
public class BetaControllerNotFoundException(string message)
    : Exception(message);
