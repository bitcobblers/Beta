namespace Beta.TestAdapter;

/// <summary>
///     Defines an adapter for the execution engine.
/// </summary>
public interface IEngineAdapter
{
    /// <summary>
    ///     Gets an instance to the underlying controller.
    /// </summary>
    /// <returns>An instance to the underlying controller.</returns>
    IEngineController? GetController();
}
