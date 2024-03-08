using System.Reflection;

namespace Beta.Discovery;

public class TestDiscoverer(
    ICoreLogger logger,
    IAssemblySourceFilter sourceFilter)
{
    public TestDiscoverer(ICoreLogger logger)
        : this(
            logger,
            new DefaultAssemblySourceFilter(
                new NetCoreFrameworkMatcher()))
    {
    }

    public IEnumerable<BetaTest> DiscoverTests(IEnumerable<string> sources, string? framework) =>
        from source in sources
        let assemblyName = Path.GetFileName(source)
        where sourceFilter.ShouldInclude(source, framework)
        from test in ScanAssembly(source, logger.CreateScope(assemblyName))
        select test;

    public IEnumerable<BetaTest> ScanAssembly(string source, ICoreLogger scanLogger)
    {
        var assembly = Assembly.LoadFrom(source);

        scanLogger.Log(LogLevel.Info, "Scanning assembly for tests");

        foreach (var discoveredTest in from type in assembly.GetTypes()
                                       where type.IsPublic && type.IsClass && type.IsAssignableTo(typeof(TestContainer))
                                       let container = (TestContainer?)Activator.CreateInstance(type)
                                       where container is not null
                                       from test in container.Discover()
                                       select test)
        {
            scanLogger.Log(LogLevel.Info, $"Discovered test: {discoveredTest.TestName}");
            yield return discoveredTest;
        }
    }
}
