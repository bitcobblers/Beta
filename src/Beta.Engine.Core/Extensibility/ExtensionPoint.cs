using Beta.Engine.Exceptions;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Extensibility;

/// <summary>
///     An ExtensionPoint represents a single point in the TestEngine
///     that may be extended by user addins and extensions.
/// </summary>
public class ExtensionPoint : IExtensionPoint
{
    /// <summary>
    ///     Construct an ExtensionPoint
    /// </summary>
    /// <param name="path">String that uniquely identifies the extension point.</param>
    /// <param name="type">Required type of any extension object.</param>
    public ExtensionPoint(string path, Type type)
    {
        Path = path;
        TypeName = type.FullName;
        Extensions = new List<ExtensionNode>();
    }

    /// <summary>
    ///     Gets a list of ExtensionNodes for extensions installed on this extension point.
    /// </summary>
    public List<ExtensionNode> Extensions { get; }

    /// <summary>
    ///     Gets the unique path identifying this extension point.
    /// </summary>
    public string Path { get; }

    /// <summary>
    ///     Gets and sets the optional description of this extension point.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets the FullName of the Type required for any extension to be installed at this extension point.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    ///     Gets an enumeration of IExtensionNodes for extensions installed on this extension point.
    /// </summary>
    IEnumerable<IExtensionNode> IExtensionPoint.Extensions => Extensions.ToArray();

    /// <summary>
    ///     Install an extension at this extension point. If the
    ///     extension node does not meet the requirements for
    ///     this extension point, an exception is thrown.
    /// </summary>
    public void Install(ExtensionNode node)
    {
        if (node.Path != Path)
        {
            var msg = string.Format("Non-matching extension path. Expected {0} but got {1}.", Path, node.Path);
            throw new BetaEngineException(msg);
        }

        // TODO: Verify that the type is correct using Cecil or Reflection
        // depending on whether the assembly is pre-loaded. For now, it's not
        // simple to verify the type without loading the extension, so we
        // let it throw at the time the object is accessed.
        Extensions.Add(node);
    }

    /// <summary>
    ///     Removes an extension from this extension point. If the
    ///     extension object is not present, the method returns
    ///     without error.
    /// </summary>
    public void Remove(ExtensionNode extension)
    {
        Extensions.Remove(extension);
    }
}
