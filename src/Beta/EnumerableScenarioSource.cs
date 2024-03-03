using System.Collections;

namespace Beta;

[PublicAPI]
public class EnumerableScenarioSource<T>(IEnumerable<T> scenarios) : IScenarioSource<T>
{
    public bool SupportsEagerLoading => true;

    public IEnumerator<T> GetEnumerator() => scenarios.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
