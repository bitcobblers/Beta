using Beta.TestAdapter.Models;

#pragma warning disable CS8618

namespace Beta.TestAdapter;

/// <summary>
///     Wraps an instance of the engine controller and calls it via reflection.
/// </summary>
/// <param name="instance">The instance to wrap.</param>
public class WrappedEngineController(object instance) : IEngineController
{
    /// <inheritdoc />
    public IEnumerable<DiscoveredTest> Query() =>
        Enumerable.Empty<DiscoveredTest>();
}
