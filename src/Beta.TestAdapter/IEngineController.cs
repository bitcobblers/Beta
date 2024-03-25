using Beta.TestAdapter.Models;

namespace Beta.TestAdapter;

public interface IEngineController
{
    IEnumerable<DiscoveredTest> Query();
}
