using System.Collections;

namespace Beta;

public class RawEnumerableTestDataSource<T>(IEnumerable<T> scenarios) : ITestDataSource<T>
{
    public bool SupportsEagerLoading => true;
    public IEnumerator<T> GetEnumerator() => scenarios.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}