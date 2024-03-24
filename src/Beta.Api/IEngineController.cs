using Beta.Api.Contracts;

namespace Beta.Api;

/// <summary>
///     Defines a common interface for engine controllers.
/// </summary>
public interface IEngineController
{
    /// <summary>
    ///     Queries for discovered tests.
    /// </summary>
    /// <returns>A collection of discovered tests.</returns>
    IEnumerable<DiscoveredTest> Query();
}
