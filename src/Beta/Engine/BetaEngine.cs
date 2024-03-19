using System.Reflection;
using System.Xml.Linq;

namespace Beta.Engine;

public class BetaEngine(Assembly assembly)
{
    public IEnumerable<XElement> Discover() => new List<XElement>();

    public void Run()
    {
    }

    public void Stop()
    {
    }
}
