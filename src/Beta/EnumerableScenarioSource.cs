using System.Collections;

namespace Beta;

[PublicAPI]
#pragma warning disable CS9113 // Parameter is unread.
public class EnumerableScenarioSource<T>(IEnumerable<T> scenarios) : IScenarioSource<T>
#pragma warning restore CS9113 // Parameter is unread.
{
    public bool SupportsEagerLoading => true;

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}