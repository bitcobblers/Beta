using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Exceptions;

/// <summary>
///     NUnitEngineException is thrown when the engine has been
///     called with improper values or when a particular facility
///     is not available.
/// </summary>
[Serializable]
public class BetaEngineException : Exception
{
    /// <summary>
    ///     Construct with a message
    /// </summary>
    public BetaEngineException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Construct with a message and inner exception
    /// </summary>
    public BetaEngineException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Serialization constructor
    /// </summary>
    public BetaEngineException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}