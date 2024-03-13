using SIO = System.IO;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Internal.FileSystemAccess.Default;

/// <summary>
///     Default implementation of <see cref="IDirectory" /> that relies on <see cref="System.IO" />.
/// </summary>
internal sealed class Directory : IDirectory
{
    private readonly DirectoryInfo _directory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Directory" /> class.
    /// </summary>
    /// <param name="path">Path of the directory.</param>
    /// <exception cref="System.Security.SecurityException">
    ///     The caller does not have the required permission to access
    ///     <paramref name="path" />.
    /// </exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
    /// <exception cref="System.ArgumentException">
    ///     <paramref name="path" /> contains invalid characters (see
    ///     <see cref="SIO.Path.GetInvalidPathChars" /> for details).
    /// </exception>
    /// <exception cref="SIO.PathTooLongException"><paramref name="path" /> exceeds the system-defined maximum length.</exception>
    public Directory(string? path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (path.IndexOfAny(Path.GetInvalidPathChars()) > -1)
        {
            throw new ArgumentException("Path contains invalid characters.", nameof(path));
        }

        _directory = new DirectoryInfo(path);

        Parent = _directory.Parent == null ? null : new Directory(_directory.Parent.FullName);
    }

    /// <inheritdoc />
    public IDirectory? Parent { get; }

    /// <inheritdoc />
    public string FullName => _directory.FullName;

    /// <inheritdoc />
    public IEnumerable<IFile> GetFiles(string? searchPattern)
    {
        if (searchPattern == null)
        {
            throw new ArgumentNullException(nameof(searchPattern));
        }

        return from currentFile in _directory.GetFiles(searchPattern)
               select new File(currentFile.FullName);
    }

    /// <inheritdoc />
    public IEnumerable<IDirectory> GetDirectories(string? searchPattern, SearchOption searchOption)
    {
        if (searchPattern == null)
        {
            throw new ArgumentNullException(nameof(searchPattern));
        }

        return from currentDirectory in _directory.GetDirectories(searchPattern, searchOption)
               select new Directory(currentDirectory.FullName);
    }
}
