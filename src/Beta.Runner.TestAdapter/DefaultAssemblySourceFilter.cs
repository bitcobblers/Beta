namespace Beta.Runner.TestAdapter;

public class DefaultAssemblySourceFilter(IFrameworkMatcher matcher) : IAssemblySourceFilter
{
    private static readonly HashSet<string> PlatformAssemblies = new(StringComparer.OrdinalIgnoreCase)
    {
        "microsoft.visualstudio.testplatform.unittestframework.dll",
        "microsoft.visualstudio.testplatform.core.dll",
        "microsoft.visualstudio.testplatform.testexecutor.core.dll",
        "microsoft.visualstudio.testplatform.extensions.msappcontaineradapter.dll",
        "microsoft.visualstudio.testplatform.objectmodel.dll",
        "microsoft.visualstudio.testplatform.utilities.dll",
        "vstest.executionengine.appcontainer.exe",
        "vstest.executionengine.appcontainer.x86.exe",

        "beta.testadapter.dll"
    };

    public bool ShouldInclude(string assemblyPath, RunSettings settings)
    {
        return !PlatformAssemblies.Contains(Path.GetFileName(assemblyPath)) &&
               matcher.IsMatch(settings.TargetFrameworkVersion) &&
               File.Exists(assemblyPath);
    }
}
