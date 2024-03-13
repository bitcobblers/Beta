using System.Reflection;
using Beta.Engine.Extensibility;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Tests.Extensibility;

public class ExtensionAssemblyTests
{
    private static readonly Assembly ThisAssembly = Assembly.GetExecutingAssembly();
    private static readonly string ThisAssemblyPath = ThisAssembly.Location;
    private static readonly string ThisAssemblyFullName = ThisAssembly.GetName().FullName;
    private static readonly string? ThisAssemblyName = ThisAssembly.GetName().Name;
    private static readonly Version? ThisAssemblyVersion = ThisAssembly.GetName().Version;

    private readonly ExtensionAssembly _ea = new(ThisAssemblyPath, false);

    [Fact]
    public void AssemblyDefinition()
    {
        _ea.Assembly.FullName.ShouldBe(ThisAssemblyFullName);
    }

    [Fact]
    public void MainModule()
    {
        _ea.MainModule.Assembly.FullName.ShouldBe(ThisAssemblyFullName);
    }

    [Fact]
    public void AssemblyName()
    {
        _ea.AssemblyName.ShouldBe(ThisAssemblyName);
    }

    [Fact]
    public void AssemblyVersion()
    {
        _ea.AssemblyVersion.ShouldBe(ThisAssemblyVersion);
    }
}
