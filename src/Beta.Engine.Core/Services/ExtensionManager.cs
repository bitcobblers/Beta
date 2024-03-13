using System.Reflection;
using Beta.Engine.Api.Extensibility;
using Beta.Engine.Exceptions;
using Beta.Engine.Extensibility;
using Beta.Engine.Internal;
using Beta.Engine.Internal.FileSystemAccess;
using Beta.Engine.Internal.FileSystemAccess.Default;
using TestCentric.Metadata;
using Backports = Beta.Engine.Internal.Backports;

namespace Beta.Engine.Services;

public sealed class ExtensionManager : IDisposable
{
    private static readonly Logger Log = InternalTrace.GetLogger(typeof(ExtensionService));
    private static readonly Version EngineVersion = typeof(ExtensionService).Assembly.GetName().Version;

    private readonly IAddinsFileReader _addinsReader;
    private readonly List<ExtensionAssembly> _assemblies = new();
    private readonly IDirectoryFinder _directoryFinder;

    private readonly List<ExtensionPoint> _extensionPoints = new();
    private readonly List<ExtensionNode> _extensions = new();

    private readonly IFileSystem _fileSystem;
    private readonly Dictionary<string, ExtensionPoint> _pathIndex = new();
    private readonly Dictionary<string, object> _visited = new();

    public ExtensionManager()
        : this(new AddinsFileReader(), new FileSystem())
    {
    }

    internal ExtensionManager(IAddinsFileReader addinsReader, IFileSystem fileSystem)
        : this(addinsReader, fileSystem, new DirectoryFinder())
    {
    }

    internal ExtensionManager(IAddinsFileReader addinsReader, IFileSystem fileSystem, IDirectoryFinder directoryFinder)
    {
        _addinsReader = addinsReader;
        _fileSystem = fileSystem;
        _directoryFinder = directoryFinder;
    }

    /// <summary>
    ///     Gets an enumeration of all ExtensionPoints in the engine.
    /// </summary>
    public IEnumerable<IExtensionPoint> ExtensionPoints => _extensionPoints.ToArray();

    /// <summary>
    ///     Gets an enumeration of all installed Extensions.
    /// </summary>
    public IEnumerable<IExtensionNode> Extensions => _extensions.ToArray();

    public void Dispose()
    {
        // Make sure all assemblies release the underlying file streams.
        foreach (var assembly in _assemblies)
        {
            assembly.Dispose();
        }
    }

    internal void FindExtensions(string startDir)
    {
        // Create the list of possible extension assemblies,
        // eliminating duplicates, start in the provided directory.
        FindExtensionAssemblies(_fileSystem.GetDirectory(startDir));

        // Check each assembly to see if it contains extensions
        foreach (var candidate in _assemblies)
        {
            FindExtensionsInAssembly(candidate);
        }
    }

    /// <summary>
    ///     Enable or disable an extension
    /// </summary>
    public void EnableExtension(string typeName, bool enabled)
    {
        foreach (var node in _extensions.Where(node => node.TypeName == typeName))
        {
            node.Enabled = enabled;
        }
    }

    /// <summary>
    ///     Get an ExtensionPoint based on its unique identifying path.
    /// </summary>
    public ExtensionPoint? GetExtensionPoint(string path)
    {
        return _pathIndex.TryGetValue(path, out var extension) ? extension : null;
    }

    /// <summary>
    ///     Get an ExtensionPoint based on the required Type for extensions.
    /// </summary>
    public ExtensionPoint? GetExtensionPoint(Type type)
    {
        return _extensionPoints.FirstOrDefault(ep => ep.TypeName == type.FullName);
    }

    /// <summary>
    ///     Get an ExtensionPoint based on a Cecil TypeReference.
    /// </summary>
    public ExtensionPoint? GetExtensionPoint(TypeReference type)
    {
        return _extensionPoints.FirstOrDefault(ep => ep.TypeName == type.FullName);
    }

    public IEnumerable<ExtensionNode> GetExtensionNodes(string path)
    {
        var ep = GetExtensionPoint(path);

        if (ep == null)
        {
            yield break;
        }

        foreach (var node in ep.Extensions)
        {
            yield return node;
        }
    }

    public ExtensionNode? GetExtensionNode(string path)
    {
        var ep = GetExtensionPoint(path);

        return ep is { Extensions.Count: > 0 } ? ep.Extensions[0] : null;
    }

    public IEnumerable<ExtensionNode> GetExtensionNodes<T>(bool includeDisabled = false)
    {
        var ep = GetExtensionPoint(typeof(T));

        if (ep == null)
        {
            yield break;
        }

        foreach (var node in ep.Extensions.Where(node => includeDisabled || node.Enabled))
        {
            yield return node;
        }
    }

    public IEnumerable<T> GetExtensions<T>()
    {
        return GetExtensionNodes<T>().Select(node => (T)node.ExtensionObject);
    }

    /// <summary>
    ///     Find the extension points in a loaded assembly.
    /// </summary>
    public void FindExtensionPoints(params Assembly[] targetAssemblies)
    {
        foreach (var assembly in targetAssemblies)
        {
            Log.Info("Scanning {0} assembly for extension points", assembly.GetName().Name);

            foreach (ExtensionPointAttribute attr in assembly.GetCustomAttributes(typeof(ExtensionPointAttribute),
                         false))
            {
                if (_pathIndex.ContainsKey(attr.Path))
                {
                    var msg = $"The Path {attr.Path} is already in use for another extension point.";
                    throw new BetaEngineException(msg);
                }

                var ep = new ExtensionPoint(attr.Path, attr.Type)
                {
                    Description = attr.Description
                };

                _extensionPoints.Add(ep);
                _pathIndex.Add(ep.Path, ep);

                Log.Info("  Found Path={0}, Type={1}", ep.Path, ep.TypeName);
            }

            foreach (var type in assembly.GetExportedTypes())
            {
                foreach (TypeExtensionPointAttribute attr in type.GetCustomAttributes(
                             typeof(TypeExtensionPointAttribute), false))
                {
                    var path = !string.IsNullOrWhiteSpace(attr.Path)
                        ? attr.Path
                        : $"/Beta/Engine/TypeExtensions/{type.Name}";

                    if (_pathIndex.ContainsKey(path))
                    {
                        var msg = $"The Path {attr.Path} is already in use for another extension point.";
                        throw new BetaEngineException(msg);
                    }

                    var ep = new ExtensionPoint(path, type)
                    {
                        Description = attr.Description
                    };

                    _extensionPoints.Add(ep);
                    _pathIndex.Add(path, ep);

                    Log.Info("  Found Path={0}, Type={1}", ep.Path, ep.TypeName);
                }
            }
        }
    }

    /// <summary>
    ///     Deduce the extension point based on the Type of an extension.
    ///     Returns null if no extension point can be found that would
    ///     be satisfied by the provided Type.
    /// </summary>
    private ExtensionPoint? DeduceExtensionPointFromType(TypeReference typeRef)
    {
        while (true)
        {
            var ep = GetExtensionPoint(typeRef);
            if (ep != null)
            {
                return ep;
            }

            var typeDef = typeRef.Resolve();

            foreach (var iface in typeDef.Interfaces)
            {
                ep = DeduceExtensionPointFromType(iface.InterfaceType);

                if (ep != null)
                {
                    return ep;
                }
            }

            var baseType = typeDef.BaseType;

            if (baseType == null || baseType.FullName == "System.Object")
            {
                return null;
            }

            typeRef = baseType;
        }
    }

    /// <summary>
    ///     Find candidate extension assemblies starting from a
    ///     given base directory.
    /// </summary>
    /// <param name="startDir"></param>
    private void FindExtensionAssemblies(IDirectory startDir)
    {
        // First check the directory itself
        ProcessAddinsFiles(startDir, false);
    }

    /// <summary>
    ///     Scans a directory for candidate addin assemblies. Note that assemblies in
    ///     the directory are only scanned if no file of type .addins is found. If such
    ///     a file is found, then those assemblies it references are scanned.
    /// </summary>
    private void ProcessDirectory(IDirectory startDir, bool fromWildCard)
    {
        var directoryName = startDir.FullName;
        if (WasVisited(startDir.FullName, fromWildCard))
        {
            Log.Warning($"Skipping directory '{directoryName}' because it was already visited.");
        }
        else
        {
            Log.Info($"Scanning directory '{directoryName}' for extensions.");
            MarkAsVisited(directoryName, fromWildCard);

            if (ProcessAddinsFiles(startDir, fromWildCard) != 0)
            {
                return;
            }

            foreach (var file in startDir.GetFiles("*.dll"))
            {
                ProcessCandidateAssembly(file.FullName, true);
            }
        }
    }

    /// <summary>
    ///     Process all .addins files found in a directory
    /// </summary>
    private int ProcessAddinsFiles(IDirectory startDir, bool fromWildCard)
    {
        var addinsFiles = startDir.GetFiles("*.addins").ToArray();
        var addinsFileCount = 0;

        if (!addinsFiles.Any())
        {
            return addinsFileCount;
        }

        foreach (var file in addinsFiles)
        {
            ProcessAddinsFile(startDir, file, fromWildCard);
            addinsFileCount += 1;
        }

        return addinsFileCount;
    }

    /// <summary>
    ///     Process a .addins type file. The file contains one entry per
    ///     line. Each entry may be a directory to scan, an assembly
    ///     path or a wildcard pattern used to find assemblies. Blank
    ///     lines and comments started by # are ignored.
    /// </summary>
    private void ProcessAddinsFile(IDirectory baseDir, IFile addinsFile, bool fromWildCard)
    {
        Log.Info("Processing file " + addinsFile.FullName);

        foreach (var entry in _addinsReader.Read(addinsFile))
        {
            var isWild = fromWildCard || entry.Contains("*");
            var args = GetBaseDirAndPattern(baseDir, entry);
            if (entry.EndsWith("/"))
            {
                foreach (var dir in _directoryFinder.GetDirectories(args.Item1, args.Item2))
                {
                    ProcessDirectory(dir, isWild);
                }
            }
            else
            {
                foreach (var file in _directoryFinder.GetFiles(args.Item1, args.Item2))
                {
                    ProcessCandidateAssembly(file.FullName, isWild);
                }
            }
        }
    }

    private Tuple<IDirectory?, string> GetBaseDirAndPattern(IDirectory baseDir, string path)
    {
        if (!Backports.Path.IsPathFullyQualified(path))
        {
            return new Tuple<IDirectory?, string>(baseDir, path);
        }

        if (path.EndsWith("/"))
        {
            return new Tuple<IDirectory?, string>(_fileSystem.GetDirectory(path), string.Empty);
        }

        return new Tuple<IDirectory?, string>(
            _fileSystem.GetDirectory(Path.GetDirectoryName(path)),
            Path.GetFileName(path));
    }

    private void ProcessCandidateAssembly(string filePath, bool fromWildCard)
    {
        if (WasVisited(filePath, fromWildCard))
        {
            return;
        }

        MarkAsVisited(filePath, fromWildCard);

        try
        {
            var candidate = new ExtensionAssembly(filePath, fromWildCard);

            if (!CanLoadTargetFramework(Assembly.GetEntryAssembly(), candidate))
            {
                return;
            }

            for (var i = 0; i < _assemblies.Count; i++)
            {
                var assembly = _assemblies[i];

                if (!candidate.IsDuplicateOf(assembly))
                {
                    continue;
                }

                if (candidate.IsBetterVersionOf(assembly))
                {
                    _assemblies[i] = candidate;
                }

                return;
            }

            _assemblies.Add(candidate);
        }
        catch (BadImageFormatException e)
        {
            if (!fromWildCard)
            {
                throw new BetaEngineException($"Specified extension {filePath} could not be read", e);
            }
        }
        catch (BetaEngineException)
        {
            if (!fromWildCard)
            {
                throw;
            }
        }
    }

    private bool WasVisited(string filePath, bool fromWildcard)
    {
        return _visited.ContainsKey($"path={filePath}_visited={fromWildcard}");
    }

    private void MarkAsVisited(string filePath, bool fromWildcard)
    {
        _visited.Add($"path={filePath}_visited={fromWildcard}", null);
    }

    /// <summary>
    ///     Scan a single assembly for extensions marked by ExtensionAttribute.
    ///     For each extension, create an ExtensionNode and link it to the
    ///     correct ExtensionPoint. Public for testing.
    /// </summary>
    internal void FindExtensionsInAssembly(ExtensionAssembly assembly)
    {
        Log.Info($"Scanning {assembly.FilePath} for Extensions");

        if (!CanLoadTargetFramework(Assembly.GetEntryAssembly(), assembly))
        {
            Log.Info($"{assembly.FilePath} cannot be loaded on this runtime");
            return;
        }

        IRuntimeFramework assemblyTargetFramework = null;

        foreach (var type in assembly.MainModule.GetTypes())
        {
            var extensionAttr = type.GetAttribute("NUnit.Engine.Extensibility.ExtensionAttribute");

            if (extensionAttr == null)
            {
                continue;
            }

            var versionArg = extensionAttr.GetNamedArgument("EngineVersion");
            if (versionArg != null && new Version((string)versionArg) > EngineVersion)
            {
                continue;
            }

            var node = new ExtensionNode(assembly.FilePath, assembly.AssemblyVersion, type.FullName,
                assemblyTargetFramework)
            {
                Path = extensionAttr.GetNamedArgument("Path") as string,
                Description = extensionAttr.GetNamedArgument("Description") as string
            };

            var enabledArg = extensionAttr.GetNamedArgument("Enabled");
            node.Enabled = enabledArg == null || (bool)enabledArg;

            Log.Info("  Found ExtensionAttribute on Type " + type.Name);

            foreach (var attr in type.GetAttributes("NUnit.Engine.Extensibility.ExtensionPropertyAttribute"))
            {
                if (attr.ConstructorArguments[0].Value is not string name ||
                    attr.ConstructorArguments[1].Value is not string value)
                {
                    continue;
                }

                node.AddProperty(name, value);
                Log.Info("        ExtensionProperty {0} = {1}", name, value);
            }

            _extensions.Add(node);

            ExtensionPoint ep;
            if (node.Path == null)
            {
                ep = DeduceExtensionPointFromType(type);
                if (ep == null)
                {
                    var msg = string.Format(
                        "Unable to deduce ExtensionPoint for Type {0}. Specify Path on ExtensionAttribute to resolve.",
                        type.FullName);
                    throw new BetaEngineException(msg);
                }

                node.Path = ep.Path;
            }
            else
            {
                ep = GetExtensionPoint(node.Path);
                if (ep == null)
                {
                    var msg = string.Format(
                        "Unable to locate ExtensionPoint for Type {0}. The Path {1} cannot be found.",
                        type.FullName,
                        node.Path);
                    throw new BetaEngineException(msg);
                }
            }

            ep.Install(node);
        }
    }

    /// <summary>
    ///     Checks that the target framework of the current runner can load the extension assembly. For example, .NET Core
    ///     cannot load .NET Framework assemblies and vice-versa.
    /// </summary>
    /// <param name="runnerAsm">The executing runner</param>
    /// <param name="extensionAsm">The extension we are attempting to load</param>
    internal static bool CanLoadTargetFramework(Assembly? runnerAsm, ExtensionAssembly extensionAsm)
    {
        if (runnerAsm == null)
        {
            return true;
        }

        var extensionFrameworkName = AssemblyDefinition.ReadAssembly(extensionAsm.FilePath).GetFrameworkName();
        var runnerFrameworkName = AssemblyDefinition.ReadAssembly(runnerAsm.Location).GetFrameworkName();
        if (runnerFrameworkName?.StartsWith(".NETStandard") == true)
        {
            throw new BetaEngineException(
                $"{runnerAsm.FullName} test runner must target .NET Core or .NET Framework, not .NET Standard");
        }

        if (runnerFrameworkName?.StartsWith(".NETCoreApp") == true)
        {
            if (extensionFrameworkName?.StartsWith(".NETStandard") == true ||
                extensionFrameworkName?.StartsWith(".NETCoreApp") == true)
            {
                return true;
            }

            Log.Info($".NET Core runners require .NET Core or .NET Standard extension for {extensionAsm.FilePath}");
            return false;
        }

        if (extensionFrameworkName?.StartsWith(".NETCoreApp") != true)
        {
            return true;
        }

        Log.Info($".NET Framework runners cannot load .NET Core extension {extensionAsm.FilePath}");
        return false;
    }
}
