using System.Diagnostics.CodeAnalysis;

namespace Beta.TestAdapter.Exceptions;

/// <summary>
///     Defines an exception that is thrown when the controller fails to load.
/// </summary>
/// <param name="message">A message explaining why the controller failed to load.</param>
/// <param name="innerException">The inner exception.</param>
[ExcludeFromCodeCoverage]
public class BetaEngineLoadFailedException(string message, Exception innerException)
    : Exception(message, innerException);
