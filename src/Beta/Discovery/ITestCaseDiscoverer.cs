using System.Reflection;

namespace Beta.Discovery;

public interface ITestCaseDiscoverer
{
    IEnumerable<Test> Discover(MethodInfo method);
}
