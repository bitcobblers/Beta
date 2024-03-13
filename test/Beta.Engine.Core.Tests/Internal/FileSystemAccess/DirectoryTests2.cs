using BIO = Beta.Engine.Internal.FileSystemAccess.Default;

namespace Beta.Engine.Core.Tests.Internal.FileSystemAccess;

/// <summary>
///     Tests the implementation of <see cref="Directory" />.
/// </summary>
/// <remarks>All tests in this fixture modify the file-system.</remarks>
[Trait("Category", "WritesToDisk")]
public sealed class DirectoryTests2 : IDisposable
{
    private readonly string[] _subDirectories;
    private readonly string _testDirectory;

    public DirectoryTests2()
    {
        _testDirectory =
            Combine(
                Path.GetTempPath(),
                "beta.engine.tests.temp",
                Guid.NewGuid().ToString());

        _subDirectories =
        [
            Combine(_testDirectory, "abc"),
            Combine(_testDirectory, "abc", "123"),
            Combine(_testDirectory, "abc", "456"),
            Combine(_testDirectory, "abc", "789"),
            Combine(_testDirectory, "abc", "789", "xyz"),
            Combine(_testDirectory, "def"),
            Combine(_testDirectory, "def", "kek"),
            Combine(_testDirectory, "def", "kek", "lel"),
            Combine(_testDirectory, "def", "kek", "lel", "mem"),
            Combine(_testDirectory, "ghi")
        ];

        Directory.CreateDirectory(_testDirectory);

        foreach (var directory in _subDirectories)
        {
            Directory.CreateDirectory(directory);
        }
    }

    //[OneTimeTearDown]
    public void Dispose()
    {
        Directory.Delete(_testDirectory, true);
    }

    private static string Combine(params string[] parts)
    {
        var result = parts[0];

        for (var i = 1; i < parts.Length; i++)
        {
            result = Path.Combine(result, parts[i]);
        }

        return result;
    }

    [Fact]
    public void GetDirectories()
    {
        var expected = new[]
        {
            Combine(_testDirectory, "abc"),
            Combine(_testDirectory, "def"),
            Combine(_testDirectory, "ghi")
        };

        var directory = new BIO.Directory(_testDirectory);

        var actualDirectories = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);
        var actual = actualDirectories.Select(x => x.FullName);

        actual.ShouldBe(expected);
    }

    [Fact]
    public void GetDirectories_AllSubDirectories()
    {
        var expected = _subDirectories.OrderBy(x => x);
        var directory = new BIO.Directory(_testDirectory);

        var actualDirectories = directory.GetDirectories("*", SearchOption.AllDirectories);
        var actual = actualDirectories.Select(x => x.FullName).OrderBy(x => x);

        actual.ShouldBe(expected);
    }

    [Fact]
    public void GetDirectories_WithPattern()
    {
        var expected = new[] { Combine(_testDirectory, "abc") };
        var directory = new BIO.Directory(_testDirectory);

        var actual =
            from dir in directory.GetDirectories("a??", SearchOption.TopDirectoryOnly)
            select dir.FullName;

        actual.ShouldBe(expected);
    }

    [Fact]
    public void GetDirectories_WithPattern_NoMatch()
    {
        var directory = new BIO.Directory(_testDirectory);

        var actual =
            from dir in directory.GetDirectories("z*", SearchOption.AllDirectories)
            select dir;

        actual.ShouldBeEmpty();
    }

    [Fact]
    public void GetDirectories_WithPattern_AllSubDirectories()
    {
        var expected = new[]
        {
            Combine(_testDirectory, "def"),
            Combine(_testDirectory, "def", "kek"),
            Combine(_testDirectory, "def", "kek", "lel"),
            Combine(_testDirectory, "def", "kek", "lel", "mem")
        }.OrderBy(x => x);

        var directory = new BIO.Directory(_testDirectory);

        var actual =
            from dir in directory.GetDirectories("?e?", SearchOption.AllDirectories)
            orderby dir.FullName
            select dir.FullName;

        actual.ShouldBe(expected);
    }
}
