namespace Beta.Engine.Api.Extensibility;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
public class TypeExtensionPointAttribute(string path) : Attribute
{
    public TypeExtensionPointAttribute() : this(string.Empty)
    {
    }

    /// <summary>
    ///     Gets the path of the extension point.  This is formatted using a '/'.
    /// </summary>
    public string Path { get; } = path;

    /// <summary>
    ///     Gets the description of the extension point.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
