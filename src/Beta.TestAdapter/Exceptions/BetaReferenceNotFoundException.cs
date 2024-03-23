using System.Diagnostics.CodeAnalysis;

namespace Beta.TestAdapter.Exceptions;

/// <summary>
///     Defines an exception that is thrown when the Beta reference is not found.
/// </summary>
/// <param name="message">A message explaining why the reference was not found</param>
[ExcludeFromCodeCoverage]
public class BetaReferenceNotFoundException(string message)
    : Exception(message);
