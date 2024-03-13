using TestCentric.Metadata;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Extensibility;

internal class ExtensionAssembly : IExtensionAssembly, IDisposable
{
    public ExtensionAssembly(string filePath, bool fromWildCard)
    {
        FilePath = filePath;
        FromWildCard = fromWildCard;
        Assembly = GetAssemblyDefinition();
    }

    public string FilePath { get; }
    public AssemblyDefinition Assembly { get; }
    public ModuleDefinition MainModule => Assembly.MainModule;

    public void Dispose()
    {
        Assembly.Dispose();
    }

    public bool FromWildCard { get; }
    public string AssemblyName => Assembly.Name.Name;
    public Version AssemblyVersion => Assembly.Name.Version;

    private AssemblyDefinition GetAssemblyDefinition()
    {
        var resolver = new DefaultAssemblyResolver();
        resolver.AddSearchDirectory(Path.GetDirectoryName(FilePath));
        resolver.AddSearchDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

        var parameters = new ReaderParameters { AssemblyResolver = resolver };
        return AssemblyDefinition.ReadAssembly(FilePath, parameters);
    }
}
