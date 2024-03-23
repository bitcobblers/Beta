using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Beta.TestAdapter;

/// <summary>
///     Defines an adapter for the execution engine.
/// </summary>
public interface IEngineAdapter : IDisposable
{
    /// <summary>
    ///     Queries the controller for discovered tests.
    /// </summary>
    /// <returns>A collection of tests.</returns>
    IEnumerable<TestCase> Query();

    /// <summary>
    ///     Instructs the controller to begin executing tests.
    /// </summary>
    void Run();

    /// <summary>
    ///     Instructs the controller to stop executing tests.
    /// </summary>
    void Stop();
}
