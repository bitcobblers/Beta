// ReSharper disable once CheckNamespace

using System.Reflection;

namespace Beta.Engine;

public static class TestEngineActivator
{
    private const string DefaultAssemblyName = "Beta.Engine.dll";
    private const string DefaultTypeName = "Beta.Engine.TestEngine";

    internal static readonly Version DefaultMinimumVersion = new(1, 0);

    public static ITestEngine? CreateInstance()
    {
        var apiLocation = typeof(TestEngineActivator).Assembly.Location;
        var directoryName = Path.GetDirectoryName(apiLocation);
        var enginePath = directoryName == null ? DefaultAssemblyName : Path.Combine(directoryName, DefaultAssemblyName);
        var assembly = Assembly.LoadFrom(enginePath);
        var engineType = assembly.GetType(DefaultTypeName);
        return Activator.CreateInstance(engineType) as ITestEngine;
    }
}
