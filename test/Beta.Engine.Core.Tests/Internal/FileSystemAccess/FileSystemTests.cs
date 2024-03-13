using System.Reflection;
using Beta.Engine.Internal.FileSystemAccess;
using Directory = System.IO.Directory;
using BIO = Beta.Engine.Internal.FileSystemAccess.Default;

namespace Beta.Engine.Core.Tests.Internal.FileSystemAccess;

public sealed class FileSystemTests
{
    [Fact]
    public void GetDirectory()
    {
        var path = Directory.GetCurrentDirectory();
        var fileSystem = new BIO.FileSystem();

        var actual = fileSystem.GetDirectory(path);

        actual.FullName.ShouldBe(path);
    }

    [Fact]
    public void GetDirectory_DirectoryDoesNotExist()
    {
        var path = GetNonExistentDirectory();
        var fileSystem = new BIO.FileSystem();

        Should.Throw<DirectoryNotFoundException>(() => fileSystem.GetDirectory(path));
    }

    [Fact]
    public void GetFile()
    {
        var path = GetTestFileLocation();
        var parent = Path.GetDirectoryName(path);
        var fileSystem = new BIO.FileSystem();

        var file = fileSystem.GetFile(path);

        file.FullName.ShouldBe(path);
        file.Parent.FullName.ShouldBe(parent);
    }

    [Fact]
    public void GetFile_DirectoryDoesNotExist()
    {
        var path = GetNonExistentDirectory();

        path = Path.Combine(path, "foobar.file");

        var fileSystem = new BIO.FileSystem();

        Should.Throw<DirectoryNotFoundException>(() => fileSystem.GetFile(path));
    }

    [Fact]
    public void Exists_FileExists()
    {
        var path = GetTestFileLocation();
        var file = new BIO.File(path);
        var fileSystem = new BIO.FileSystem();

        var exists = fileSystem.Exists(file);

        exists.ShouldBeTrue();
    }

    [Fact]
    public void Exists_FileDoesNotExist()
    {
        var path = GetNonExistentFile();
        var file = new BIO.File(path);
        var fileSystem = new BIO.FileSystem();

        var exists = fileSystem.Exists(file);

        exists.ShouldBeFalse();
    }

    [Fact]
    public void ExistsFileIsNull()
    {
        var fileSystem = new BIO.FileSystem();

        Should.Throw<ArgumentNullException>(() => fileSystem.Exists((IFile?)null));
    }

    [Fact]
    public void Exists_DirectoryExists()
    {
        var path = Directory.GetCurrentDirectory();
        var directory = new BIO.Directory(path);
        var fileSystem = new BIO.FileSystem();

        var exists = fileSystem.Exists(directory);

        exists.ShouldBeTrue();
    }

    [Fact]
    public void Exists_DirectoryDoesNotExist()
    {
        var path = GetNonExistentDirectory();
        var directory = new BIO.Directory(path);
        var fileSystem = new BIO.FileSystem();

        var exists = fileSystem.Exists(directory);

        exists.ShouldBeFalse();
    }

    [Fact]
    public void Exists_DirectoryIsNull()
    {
        var fileSystem = new BIO.FileSystem();

        Should.Throw<ArgumentNullException>(() => fileSystem.Exists((IDirectory?)null));
    }

    private static string GetNonExistentFile()
    {
        var path = GetTestFileLocation();

        while (File.Exists(path))
        {
            path += "x";
        }

        return path;
    }

    private static string GetNonExistentDirectory()
    {
        var path = Directory.GetCurrentDirectory();

        while (Directory.Exists(path))
        {
            path += "x";
        }

        return path;
    }

    private static string GetTestFileLocation()
    {
        return Assembly.GetAssembly(typeof(FileTests))?.Location ?? string.Empty;
    }
}
