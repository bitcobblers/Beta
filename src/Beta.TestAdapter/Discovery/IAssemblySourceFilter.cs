namespace Beta.TestAdapter.Discovery;

public interface IAssemblySourceFilter
{
    public bool ShouldInclude(string assemblyPath, string? frameworkVersion);
}
