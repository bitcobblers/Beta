// ReSharper disable once CheckNamespace

namespace Beta.Engine.Extensibility;

internal interface IExtensionAssembly
{
    bool FromWildCard { get; }
    string AssemblyName { get; }
    Version AssemblyVersion { get; }
}
