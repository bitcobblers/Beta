using SIO = System.IO;

// ReSharper disable once CheckNamespace
namespace Beta.Engine.Internal.FileSystemAccess.Default;

/// <summary>
///     Default implementation of <see cref="IFileSystem" /> that relies on <see cref="System.IO" />.
/// </summary>
internal sealed class FileSystem : IFileSystem
{
    /// <inheritdoc />
    public bool Exists(IDirectory? directory)
    {
        if (directory == null)
        {
            throw new ArgumentNullException(nameof(directory));
        }

        return SIO.Directory.Exists(directory.FullName);
    }

    /// <inheritdoc />
    public bool Exists(IFile? file)
    {
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        return SIO.File.Exists(file.FullName);
    }

    /// <inheritdoc />
    public IDirectory GetDirectory(string path)
    {
        if (SIO.Directory.Exists(path))
        {
            return new Directory(path);
        }

        throw new DirectoryNotFoundException($"Directory '{path}' not found.");
    }

    /// <inheritdoc />
    public IFile GetFile(string path)
    {
        var directory = Path.GetDirectoryName(path);

        if (SIO.Directory.Exists(directory))
        {
            return new File(path);
        }

        throw new DirectoryNotFoundException($"Directory '{directory}' not found.");
    }
}
