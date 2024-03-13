using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Extensibility;

public class ExtensionNode : IExtensionNode
{
    private readonly Dictionary<string, List<string>> _properties = new();
    private object? _extensionObject;

    /// <summary>
    ///     Construct an ExtensionNode supplying the assembly path and type name.
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly where this extension is found.</param>
    /// <param name="assemblyVersion">The version of the extension assembly.</param>
    /// <param name="typeName">The full name of the Type of the extension object.</param>
    /// <param name="targetFramework">The target framework of the extension assembly.</param>
    public ExtensionNode(string assemblyPath,
                         Version assemblyVersion,
                         string typeName,
                         IRuntimeFramework targetFramework)
    {
        AssemblyPath = assemblyPath;
        AssemblyVersion = assemblyVersion;
        TypeName = typeName;
        TargetFramework = targetFramework;
        Enabled = true; // By default
    }

    /// <summary>
    ///     Gets an object of the specified extension type, loading the Assembly
    ///     and creating the object as needed. Note that this property always
    ///     returns the same object. Use CreateExtensionObject if a new one is
    ///     needed each time or to specify arguments.
    /// </summary>
    public object ExtensionObject
    {
        get { return _extensionObject ??= CreateExtensionObject(); }
    }

    /// <summary>
    ///     Gets the full name of the Type of the extension object.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    ///     The TargetFramework of the extension assembly.
    /// </summary>
    public IRuntimeFramework? TargetFramework { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="ExtensionNode" /> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    public bool Enabled { get; set; }

    /// <summary>
    ///     Gets and sets the unique string identifying the ExtensionPoint for which
    ///     this Extension is intended. This identifier may be supplied by the attribute
    ///     marking the extension or deduced by NUnit from the Type of the extension class.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    ///     An optional description of what the extension does.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     Gets a collection of the names of all this extension's properties
    /// </summary>
    public IEnumerable<string> PropertyNames => _properties.Keys;

    /// <summary>
    ///     Gets a collection of the values of a particular named property.
    ///     If none are present, returns an empty enumerator.
    /// </summary>
    /// <param name="name">The property name</param>
    /// <returns>A collection of values</returns>
    public IEnumerable<string> GetValues(string name)
    {
        if (_properties.TryGetValue(name, out var values))
        {
            return values;
        }

        return Array.Empty<string>();
    }

    /// <summary>
    ///     Gets the path to the assembly where the extension is defined.
    /// </summary>
    public string AssemblyPath { get; }

    /// <summary>
    ///     Gets the version of the extension assembly.
    /// </summary>
    public Version AssemblyVersion { get; }

    /// <summary>
    ///     Gets a newly created extension object, created in the current application domain
    /// </summary>
    public object? CreateExtensionObject(params object[] args)
    {
        var assembly = Assembly.LoadFrom(AssemblyPath);
        var typeinfo = assembly.DefinedTypes.FirstOrDefault(t => t.FullName == TypeName);

        return typeinfo == null ? null : Activator.CreateInstance(typeinfo.AsType(), args);
    }

    public void AddProperty(string name, string val)
    {
        if (_properties.TryGetValue(name, out var property))
        {
            property.Add(val);
        }
        else
        {
            var list = new List<string> { val };
            _properties.Add(name, list);
        }
    }

    public override string ToString()
    {
        return $"{TypeName} - {Path}";
    }
}
