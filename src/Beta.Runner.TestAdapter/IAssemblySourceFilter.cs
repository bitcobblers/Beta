namespace Beta.Runner.TestAdapter;

public interface IAssemblySourceFilter
{
    public bool ShouldInclude(string assemblyPath, RunSettings settings);
}
