using BIO = Beta.Engine.Internal.FileSystemAccess.Default;

namespace Beta.Engine.Core.Tests.Internal.FileSystemAccess;

public sealed class DirectoryTests
{
    [Fact]
    public void Init()
    {
        var path = Directory.GetCurrentDirectory();
        var parent = new DirectoryInfo(path).Parent?.FullName;

        var directory = new BIO.Directory(path);

        path.ShouldBe(directory.FullName);
        parent.ShouldBe(directory.Parent?.FullName);
    }

    [Fact]
    public void Init_PathIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new BIO.Directory(null));
    }

    [Fact]
    public void Init_InvalidPath()
    {
        Should.Throw<ArgumentException>(() =>
            new BIO.Directory(@"c:\this\is\an\invalid" +
                              Path.GetInvalidPathChars()[
                                  Path.GetInvalidPathChars().Length - 1] + "path"));
    }

    [Fact]
    public void Init_EmptyPath()
    {
        Should.Throw<ArgumentException>(() => new BIO.Directory(string.Empty));
    }

    [Fact]
    public void Init_TrailingDirectorySeparator()
    {
        var path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
        var parent = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.FullName;

        var directory = new BIO.Directory(path);

        path.ShouldBe(directory.FullName);
        parent.ShouldBe(directory.Parent?.FullName);
    }

    // Skip this test on non-Windows systems since System.IO.DirectoryInfo appends '\\server\share' to the current working-directory, making this test useless.
    [Fact]
    //[Platform("Win")]
    public void Init_NoParent_SMB()
    {
        var path = @"\\server\share";
        var directory = new BIO.Directory(path);

        path.ShouldBe(directory.FullName);
        directory.Parent.ShouldBeNull();
    }

    // Skip this test on non-Windows systems since System.IO.DirectoryInfo appends 'x:\' to the current working-directory, making this test useless.
    [Fact]
    //[Platform("Win")]
    public void Init_NoParent_Drive()
    {
        var path = "x:\\";
        var directory = new BIO.Directory(path);

        path.ShouldBe(directory.FullName);
        directory.Parent.ShouldBeNull();
    }

    [Fact]
    public void Init_NoParent_Root()
    {
        var path = "/";
        var expected = new DirectoryInfo(path).FullName;
        var directory = new BIO.Directory(path);

        expected.ShouldBe(directory.FullName);
        directory.Parent.ShouldBeNull();
    }

    [Fact]
    public void GetFiles()
    {
        var path = Directory.GetCurrentDirectory();
        var expected = new DirectoryInfo(path).GetFiles().Select(x => x.FullName);
        var directory = new BIO.Directory(path);

        var actualFiles = directory.GetFiles("*");
        var actual = actualFiles.Select(x => x.FullName);

        actual.ShouldBe(expected);
    }

    [Fact]
    public void GetFiles_NonExistingDirectory()
    {
        var path = Directory.GetCurrentDirectory();
        while (Directory.Exists(path))
        {
            path += "a";
        }

        var directory = new BIO.Directory(path);

        Should.Throw<DirectoryNotFoundException>(() => directory.GetFiles("*"));
    }

    [Fact]
    public void GetFiles_WithPattern()
    {
        var path = Directory.GetCurrentDirectory();
        var expected = new DirectoryInfo(path).GetFiles("*.dll").Select(x => x.FullName);
        var directory = new BIO.Directory(path);

        var actualFiles = directory.GetFiles("*.dll");
        var actual = actualFiles.Select(x => x.FullName);

        actual.ShouldBe(expected);
    }

    [Fact]
    public void GetFiles_SearchPatternIsNull()
    {
        var path = Directory.GetCurrentDirectory();
        var directory = new BIO.Directory(path);

        Should.Throw<ArgumentNullException>(() => directory.GetFiles(null));
    }

    [Fact]
    public void GetDirectories_NonExistingDirectory()
    {
        var path = Directory.GetCurrentDirectory();
        while (Directory.Exists(path))
        {
            path += "a";
        }

        var directory = new BIO.Directory(path);

        Should.Throw<DirectoryNotFoundException>(() => directory.GetDirectories("*", SearchOption.TopDirectoryOnly));
    }

    [Fact]
    public void GetDirectories_SearchPatternIsNull()
    {
        var path = Directory.GetCurrentDirectory();
        var directory = new BIO.Directory(path);

        Should.Throw<ArgumentNullException>(() => directory.GetDirectories(null, SearchOption.TopDirectoryOnly));
    }

    [Fact]
    public void GetDirectories_SearchOptionIsInvalid()
    {
        var path = Directory.GetCurrentDirectory();
        var directory = new BIO.Directory(path);

        Should.Throw<ArgumentOutOfRangeException>(() => directory.GetDirectories("*", (SearchOption)5));
    }
}
