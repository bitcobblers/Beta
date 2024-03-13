using System.Reflection;
using Beta.Engine.Extensibility;
using Beta.Engine.Internal;
using Beta.Engine.Internal.FileSystemAccess;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Services;

/// <summary>
///     The ExtensionService discovers ExtensionPoints and Extensions and
///     maintains them in a database. It can return extension nodes or
///     actual extension objects on request.
/// </summary>
public class ExtensionService : Service, IExtensionService
{
    private readonly ExtensionManager _extensionManager;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExtensionService" /> class.
    /// </summary>
    public ExtensionService()
    {
        _extensionManager = new ExtensionManager();
    }

    internal ExtensionService(IAddinsFileReader addinsReader, IFileSystem fileSystem)
        : this(addinsReader, fileSystem, new DirectoryFinder())
    {
        _extensionManager = new ExtensionManager(addinsReader, fileSystem);
    }

    internal ExtensionService(IAddinsFileReader addinsReader, IFileSystem fileSystem, IDirectoryFinder directoryFinder)
    {
        _extensionManager = new ExtensionManager(addinsReader, fileSystem, directoryFinder);
    }

    /// <inheritdoc />
    public IEnumerable<IExtensionPoint> ExtensionPoints => _extensionManager.ExtensionPoints;

    /// <inheritdoc />
    public IEnumerable<IExtensionNode> Extensions => _extensionManager.Extensions;

    /// <inheritdoc />
    IExtensionPoint IExtensionService.GetExtensionPoint(string path)
    {
        return _extensionManager.GetExtensionPoint(path);
    }

    /// <inheritdoc />
    IEnumerable<IExtensionNode> IExtensionService.GetExtensionNodes(string path)
    {
        foreach (var node in _extensionManager.GetExtensionNodes(path))
        {
            yield return node;
        }
    }

    /// <inheritdoc />
    public void EnableExtension(string typeName, bool enabled)
    {
        _extensionManager.EnableExtension(typeName, enabled);
    }

    public IEnumerable<T> GetExtensions<T>()
    {
        return _extensionManager.GetExtensions<T>();
    }

    public ExtensionNode GetExtensionNode(string path)
    {
        return _extensionManager.GetExtensionNode(path);
    }

    public IEnumerable<ExtensionNode> GetExtensionNodes<T>()
    {
        return _extensionManager.GetExtensionNodes<T>();
    }

    /// <inheritdoc />
    public override void StartService()
    {
        try
        {
            _extensionManager.FindExtensionPoints(
                Assembly.GetExecutingAssembly(),
                typeof(ITestEngine).Assembly);

            var thisAssembly = Assembly.GetExecutingAssembly();
            _extensionManager.FindExtensions(AssemblyHelper.GetDirectoryName(thisAssembly));

            Status = ServiceStatus.Started;
        }
        catch
        {
            Status = ServiceStatus.Error;
            throw;
        }
    }

    /// <inheritdoc />
    public override void StopService()
    {
        _extensionManager.Dispose();

        Status = ServiceStatus.Stopped;
    }
}
