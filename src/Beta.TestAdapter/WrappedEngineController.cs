using System.Text.Json;
using Beta.TestAdapter.Models;

#pragma warning disable CS8618

namespace Beta.TestAdapter;

/// <summary>
///     Wraps an instance of the engine controller and calls it via reflection.
/// </summary>
/// <param name="instance">The instance to wrap.</param>
public class WrappedEngineController(object instance) : IEngineController
{
    private readonly Type _instanceType = instance.GetType();

    /// <inheritdoc />
    public IEnumerable<DiscoveredTest> Query() =>
        from test in Execute<IEnumerable<string>>("Query", []) ?? []
        where test != null
        select JsonSerializer.Deserialize<DiscoveredTest>(test);

    private T? Execute<T>(string methodName, object?[] parameters)
        where T : class
    {
        var method = _instanceType.GetMethod(methodName);
        return method?.Invoke(instance, parameters) as T;
    }
}
