using System.Reflection;
using BIO = Beta.Engine.Internal.FileSystemAccess.Default;

namespace Beta.Engine.Core.Tests.Internal.FileSystemAccess;

public sealed class FileTests
{
    [Fact]
    public void Init()
    {
        var path = GetTestFileLocation();
        var parent = Path.GetDirectoryName(path);

        var file = new BIO.File(path);

        file.FullName.ShouldBe(path);
        file.Parent.FullName.ShouldBe(parent);
    }

    [Fact]
    public void Init_PathIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new BIO.File(null));
    }

    [Fact]
    public void Init_InvalidPath_InvalidDirectory()
    {
        var path = Path.GetInvalidPathChars()[Path.GetInvalidPathChars().Length - 1] + GetTestFileLocation();

        Should.Throw<ArgumentException>(() => new BIO.File(path));
    }

    [Fact]
    public void Init_InvalidPath_InvalidFileName()
    {
        var invalidCharThatIsNotInInvalidPathChars =
            Path.GetInvalidFileNameChars().Except(Path.GetInvalidPathChars()).First();
        var path = GetTestFileLocation() + invalidCharThatIsNotInInvalidPathChars;

        Should.Throw<ArgumentException>(() => new BIO.File(path));
    }

    [Fact]
    public void Init_EmptyPath()
    {
        Should.Throw<ArgumentException>(() => new BIO.File(string.Empty));
    }

    [Fact]
    public void Init_NonExistingFile()
    {
        var path = GetTestFileLocation();
        while (File.Exists(path))
        {
            path += "a";
        }

        var parent = Path.GetDirectoryName(path);
        var file = new BIO.File(path);

        file.FullName.ShouldBe(path);
        file.Parent.FullName.ShouldBe(parent);
    }

    [Fact]
    public void Init_PathIsDirectory()
    {
        var path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;

        Should.Throw<ArgumentException>(() => new BIO.File(path));
    }

    private static string? GetTestFileLocation()
    {
        return Assembly.GetAssembly(typeof(FileTests))?.Location;
    }
}
