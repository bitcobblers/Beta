using Beta.Engine.Internal;
using Beta.Engine.Internal.FileSystemAccess;

namespace Beta.Engine.Core.Tests.Internal;

public class DirectoryFinderTests
{
    private static readonly string Root = Path.DirectorySeparatorChar == '\\' ? "c:" : string.Empty;

    private readonly Dictionary<string, IDirectory> _fakedDirectories = new()
    {
        [Root] = A.Fake<IDirectory>()
    };

    private readonly Dictionary<string, IFile> _fakedFiles = new();


    public DirectoryFinderTests()
    {
        var directories = new[]
        {
            "tools/frobuscator/tests/abc",
            "tools/frobuscator/tests/def",
            "tools/metamorphosator/addins/empty",
            "tools/metamorphosator/addins/morph",
            "tools/metamorphosator/tests/v1",
            "tools/metamorphosator/tests/v1/tmp",
            "tools/metamorphosator/tests/v2"
        };

        var files = new[]
        {
            "tools/frobuscator/tests/config.cfg",
            "tools/frobuscator/tests/abc/tests.abc.dll",
            "tools/frobuscator/tests/abc/tests.123.dll",
            "tools/frobuscator/tests/def/tests.def.dll",
            "tools/metamorphosator/addins/readme.txt",
            "tools/metamorphosator/addins/morph/setup.ini",
            "tools/metamorphosator/addins/morph/code.cs",
            "tools/metamorphosator/tests/v1/test-assembly.dll",
            "tools/metamorphosator/tests/v1/test-assembly.pdb",
            "tools/metamorphosator/tests/v1/result.xml",
            "tools/metamorphosator/tests/v1/tmp/tmp.dat",
            "tools/metamorphosator/tests/v2/test-assembly.dll",
            "tools/metamorphosator/tests/v2/test-assembly.pdb",
            "tools/metamorphosator/tests/v2/result.xml"
        };

        A.CallTo(() => _fakedDirectories[Root].FullName).Returns(Root);

        foreach (var path in directories)
        {
            var parts = path.Split('/');

            for (var i = 0; i < parts.Length; i++)
            {
                var absolutePath = CreateAbsolutePath(parts.Take(i + 1));

                if (_fakedDirectories.ContainsKey(absolutePath))
                {
                    continue;
                }

                var fake = A.Fake<IDirectory>();

                A.CallTo(() => fake.FullName).Returns(absolutePath);
                A.CallTo(() => fake.Parent).Returns(i == 0
                    ? _fakedDirectories[Root]
                    : _fakedDirectories[CreateAbsolutePath(parts.Take(i))]);

                _fakedDirectories.Add(absolutePath, fake);
            }
        }

        foreach (var directory in _fakedDirectories.Values)
        {
            A.CallTo(() => directory
                 .GetDirectories("*", SearchOption.AllDirectories))
             .Returns(_fakedDirectories
                      .Where(kvp =>
                          kvp.Key.StartsWith(directory.FullName + Path.DirectorySeparatorChar))
                      .Select(kvp => kvp.Value));

            A.CallTo(() => directory
                 .GetDirectories("*", SearchOption.TopDirectoryOnly))
             .Returns(_fakedDirectories
                      .Where(kvp =>
                          kvp.Key.StartsWith(directory.FullName + Path.DirectorySeparatorChar) &&
                          kvp.Key.LastIndexOf(Path.DirectorySeparatorChar) <= directory.FullName.Length)
                      .Select(kvp => kvp.Value));
        }

        foreach (var filePath in files)
        {
            var directory = GetFakeDirectory(filePath.Split('/').Reverse().Skip(1).Reverse().ToArray());
            var file = A.Fake<IFile>();

            A.CallTo(() => file.FullName).Returns(CreateAbsolutePath(filePath.Split('/')));
            A.CallTo(() => file.Parent).Returns(directory);

            _fakedFiles.Add(file.FullName, file);
        }

        foreach (var directory in _fakedDirectories.Values)
        {
            var directoryContent = _fakedFiles.Values.Where(x => x.Parent == directory).ToArray();
            A.CallTo(() => directory.GetFiles("*")).Returns(directoryContent);
        }
    }

    [Fact]
    public void GetDirectories_Asterisk_Tools()
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools");
        var expected = new[]
                       {
                           CombinePath(baseDir.FullName, "metamorphosator"),
                           CombinePath(baseDir.FullName, "frobuscator")
                       }
                       .OrderBy(x => x).ToArray();

        var result = finder.GetDirectories(baseDir, "*");
        var actual = result.Select(x => x.FullName).OrderBy(x => x);

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetDirectories("*", SearchOption.TopDirectoryOnly)).MustHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "frobuscator").GetDirectories(A<string>._, A<SearchOption>._))
         .MustNotHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "metamorphosator").GetDirectories(A<string>._, A<SearchOption>._))
         .MustNotHaveHappened();
    }

    [Fact]
    public void GetDirectories_Asterisk_Metamorphosator()
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "metamorphosator");
        var expected = new[] { CombinePath(baseDir.FullName, "addins"), CombinePath(baseDir.FullName, "tests") };

        var result = finder.GetDirectories(baseDir, "*");
        var actual = result.Select(x => x.FullName);

        actual.ShouldBe(expected);

        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetDirectories(A<string>._, A<SearchOption>._)).MustHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "frobuscator").GetDirectories(A<string>._, A<SearchOption>._))
         .MustNotHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "metamorphosator", "addins")
            .GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "metamorphosator", "tests")
            .GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
    }

    [Fact]
    public void GetDirectories_Greedy_Tools()
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools");
        var expected = _fakedDirectories.Values.Select(x => x.FullName).Where(x => x != Root);

        var result = finder.GetDirectories(baseDir, "**");
        var actual = result.Select(x => x.FullName);

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetDirectories("*", SearchOption.AllDirectories)).MustHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "frobuscator").GetDirectories(A<string>._, A<SearchOption>._))
         .MustNotHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "metamorphosator").GetDirectories(A<string>._, A<SearchOption>._))
         .MustNotHaveHappened();
    }

    [Fact]
    public void GetDirectories_Greedy_Metamorphosator()
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "metamorphosator");
        var expected = new[]
        {
            CombinePath(baseDir.FullName),
            CombinePath(baseDir.FullName, "addins"),
            CombinePath(baseDir.FullName, "tests"),
            CombinePath(baseDir.FullName, "tests", "v1"),
            CombinePath(baseDir.FullName, "tests", "v1", "tmp"),
            CombinePath(baseDir.FullName, "tests", "v2"),
            CombinePath(baseDir.FullName, "addins", "morph"),
            CombinePath(baseDir.FullName, "addins", "empty")
        }.OrderBy(x => x).ToArray();

        var result = finder.GetDirectories(baseDir, "**");
        var actual = result.Select(x => x.FullName).OrderBy(x => x);

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetDirectories("*", SearchOption.AllDirectories)).MustHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "frobuscator").GetDirectories(A<string>._, A<SearchOption>._))
         .MustNotHaveHappened();
    }

    [InlineData("*ddi*")]
    [InlineData("addi*")]
    [InlineData("addins")]
    [Theory]
    public void GetDirectories_WordWithWildcard_NoMatch(string pattern)
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools");

        var result = finder.GetDirectories(baseDir, pattern);

        result.ShouldBeEmpty();
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetDirectories(pattern, SearchOption.TopDirectoryOnly)).MustHaveHappened();
    }

    [InlineData("*din*")]
    [InlineData("addi*")]
    [InlineData("addins")]
    [InlineData("a?dins")]
    [InlineData("a?din?")]
    [Theory]
    public void GetDirectories_WordWithWildcard_OneMatch(string pattern)
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "metamorphosator");

        A.CallTo(() => baseDir.GetDirectories(pattern, SearchOption.TopDirectoryOnly))
         .Returns(new[] { GetFakeDirectory("tools", "metamorphosator", "addins") });

        var expected = new[] { CombinePath(baseDir.FullName, "addins") };

        var result = finder.GetDirectories(baseDir, pattern);
        var actual = result.Select(x => x.FullName);

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetDirectories(pattern, SearchOption.TopDirectoryOnly)).MustHaveHappened();
    }

    [InlineData("v*")]
    [InlineData("*")]
    [InlineData("v?")]
    [Theory]
    public void GetDirectories_WordWithWildcard_MultipleMatches(string pattern)
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "metamorphosator", "tests");

        A.CallTo(() => baseDir.GetDirectories(pattern, SearchOption.TopDirectoryOnly)).Returns(new[]
        {
            GetFakeDirectory("tools", "metamorphosator", "tests", "v1"),
            GetFakeDirectory("tools", "metamorphosator", "tests", "v2")
        });

        var expected = new[] { CombinePath(baseDir.FullName, "v1"), CombinePath(baseDir.FullName, "v2") };

        var result = finder.GetDirectories(baseDir, pattern);
        var actual = result.Select(x => x.FullName);

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetDirectories(pattern, SearchOption.TopDirectoryOnly)).MustHaveHappened();
    }

    [InlineData("tests/v*")]
    [InlineData("tests/*")]
    [InlineData("tests/v?")]
    [InlineData("*/v*")]
    [InlineData("*/v?")]
    [InlineData("**/v*")]
    [InlineData("**/v?")]
    [InlineData("te*/v*")]
    [InlineData("te*/*")]
    [InlineData("te*/v?")]
    [InlineData("t?sts/v*")]
    [InlineData("t?sts/*")]
    [InlineData("t?sts/v?")]
    [InlineData("./tests/v*")]
    [InlineData("./tests/*")]
    [InlineData("./tests/v?")]
    [InlineData("./*/v*")]
    [InlineData("./*/v?")]
    [InlineData("./**/v*")]
    [InlineData("./**/v?")]
    [InlineData("./te*/v*")]
    [InlineData("./te*/*")]
    [InlineData("./te*/v?")]
    [InlineData("./t?sts/v*")]
    [InlineData("./t?sts/*")]
    [InlineData("./t?sts/v?")]
    [InlineData("**/tests/v*")]
    [InlineData("**/tests/*")]
    [InlineData("**/tests/v?")]
    [InlineData("**/*/v*")]
    [InlineData("**/*/v?")]
    [InlineData("**/te*/v*")]
    [InlineData("**/te*/*")]
    [InlineData("**/te*/v?")]
    [InlineData("**/t?sts/v*")]
    [InlineData("**/t?sts/*")]
    [InlineData("**/t?sts/v?")]
    [Theory]
    public void GetDirectories_MultipleComponents_MultipleMatches(string pattern)
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "metamorphosator");
        var testsDir = GetFakeDirectory("tools", "metamorphosator", "tests");
        var baseDirContent = new[] { testsDir };
        var testsDirContent = new[]
        {
            GetFakeDirectory("tools", "metamorphosator", "tests", "v1"),
            GetFakeDirectory("tools", "metamorphosator", "tests", "v2")
        };

        A.CallTo(() => baseDir.GetDirectories("tests", SearchOption.TopDirectoryOnly)).Returns(baseDirContent);
        A.CallTo(() => baseDir.GetDirectories("te*", SearchOption.TopDirectoryOnly)).Returns(baseDirContent);
        A.CallTo(() => baseDir.GetDirectories("t?sts", SearchOption.TopDirectoryOnly)).Returns(baseDirContent);
        A.CallTo(() => testsDir.GetDirectories("v*", SearchOption.TopDirectoryOnly)).Returns(testsDirContent);
        A.CallTo(() => testsDir.GetDirectories("*", SearchOption.TopDirectoryOnly)).Returns(testsDirContent);
        A.CallTo(() => testsDir.GetDirectories("v?", SearchOption.TopDirectoryOnly)).Returns(testsDirContent);

        var expected = new[]
            { CombinePath(baseDir.FullName, "tests", "v1"), CombinePath(baseDir.FullName, "tests", "v2") };

        var result = finder.GetDirectories(baseDir, pattern);
        var actual = result.Select(x => x.FullName);

        actual.ShouldBe(expected);
    }

    [InlineData("*/tests/v*")]
    [InlineData("**/tests/v*")]
    [Theory]
    public void GetDirectories_MultipleComponents_MultipleMatches_Asterisk(string pattern)
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools");
        var frobuscatorDir = GetFakeDirectory("tools", "frobuscator");
        var metamorphosatorDir = GetFakeDirectory("tools", "metamorphosator");
        var testsDir = GetFakeDirectory("tools", "frobuscator", "tests");
        var testsDir2 = GetFakeDirectory("tools", "metamorphosator", "tests");

        A.CallTo(() => frobuscatorDir.GetDirectories("tests", SearchOption.TopDirectoryOnly))
         .Returns(new[] { testsDir });
        A.CallTo(() => metamorphosatorDir.GetDirectories("tests", SearchOption.TopDirectoryOnly))
         .Returns(new[] { testsDir2 });
        A.CallTo(() => testsDir2.GetDirectories("v*", SearchOption.TopDirectoryOnly)).Returns(new[]
        {
            GetFakeDirectory("tools", "metamorphosator", "tests", "v1"),
            GetFakeDirectory("tools", "metamorphosator", "tests", "v2")
        });
        A.CallTo(() => testsDir2.GetDirectories("v?", SearchOption.TopDirectoryOnly)).Returns(new[]
        {
            GetFakeDirectory("tools", "metamorphosator", "tests", "v1"),
            GetFakeDirectory("tools", "metamorphosator", "tests", "v2")
        });

        var expected = new[]
        {
            CombinePath(baseDir.FullName, "metamorphosator", "tests", "v1"),
            CombinePath(baseDir.FullName, "metamorphosator", "tests", "v2")
        };

        var result = finder.GetDirectories(baseDir, pattern);
        var actual = result.Select(x => x.FullName);

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => testsDir.GetDirectories("v*", SearchOption.TopDirectoryOnly)).MustHaveHappened();
    }

    [InlineData("*/tests/v?")]
    [InlineData("**/tests/v?")]
    [Theory]
    public void GetDirectories_MultipleComponents_MultipleMatches_QuestionMark(string pattern)
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools");
        var frobuscatorDir = GetFakeDirectory("tools", "frobuscator");
        var metamorphosatorDir = GetFakeDirectory("tools", "metamorphosator");
        var testsDir = GetFakeDirectory("tools", "frobuscator", "tests");
        var testsDir2 = GetFakeDirectory("tools", "metamorphosator", "tests");

        A.CallTo(() => frobuscatorDir.GetDirectories("tests", SearchOption.TopDirectoryOnly))
         .Returns(new[] { testsDir });
        A.CallTo(() => metamorphosatorDir.GetDirectories("tests", SearchOption.TopDirectoryOnly))
         .Returns(new[] { testsDir2 });
        A.CallTo(() => testsDir2.GetDirectories("v*", SearchOption.TopDirectoryOnly)).Returns(new[]
        {
            GetFakeDirectory("tools", "metamorphosator", "tests", "v1"),
            GetFakeDirectory("tools", "metamorphosator", "tests", "v2")
        });
        A.CallTo(() => testsDir2.GetDirectories("v?", SearchOption.TopDirectoryOnly)).Returns(new[]
        {
            GetFakeDirectory("tools", "metamorphosator", "tests", "v1"),
            GetFakeDirectory("tools", "metamorphosator", "tests", "v2")
        });

        var expected = new[]
        {
            CombinePath(baseDir.FullName, "metamorphosator", "tests", "v1"),
            CombinePath(baseDir.FullName, "metamorphosator", "tests", "v2")
        };

        var result = finder.GetDirectories(baseDir, pattern);
        var actual = result.Select(x => x.FullName);

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => testsDir.GetDirectories("v?", SearchOption.TopDirectoryOnly)).MustHaveHappened();
    }

    [InlineData("./**/*")]
    [InlineData("**/*")]
    [Theory]
    public void GetDirectories_MultipleComponents_AllDirectories(string pattern)
    {
        var finder = new DirectoryFinder();
        var baseDir = _fakedDirectories[Root];
        var expected = _fakedDirectories.Values.Where(x => x != baseDir).OrderBy(x => x.FullName);

        var actual = finder.GetDirectories(baseDir, pattern).OrderBy(x => x.FullName);

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetDirectories("*", SearchOption.AllDirectories)).MustHaveHappened();

        foreach (var dir in _fakedDirectories.Values.Where(x => x != baseDir))
        {
            A.CallTo(() => dir.GetDirectories("*", SearchOption.TopDirectoryOnly)).MustHaveHappened();
        }
    }

    [Fact]
    public void GetDirectories_GreedyThenWordThenGreedy()
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools");

        A.CallTo(() =>
             GetFakeDirectory("tools", "frobuscator")
                 .GetDirectories("tests", SearchOption.TopDirectoryOnly))
         .Returns(new[]
         {
             GetFakeDirectory("tools", "frobuscator", "tests")
         });

        A.CallTo(() =>
             GetFakeDirectory("tools", "metamorphosator", "tests")
                 .GetDirectories("tests", SearchOption.TopDirectoryOnly))
         .Returns(new[]
         {
             GetFakeDirectory("tools", "metamorphosator", "tests")
         });


        var expected = new[]
        {
            GetFakeDirectory("tools", "frobuscator", "tests"),
            GetFakeDirectory("tools", "frobuscator", "tests", "abc"),
            GetFakeDirectory("tools", "frobuscator", "tests", "def"),
            GetFakeDirectory("tools", "metamorphosator", "tests"),
            GetFakeDirectory("tools", "metamorphosator", "tests", "v1"),
            GetFakeDirectory("tools", "metamorphosator", "tests", "v1", "tmp"),
            GetFakeDirectory("tools", "metamorphosator", "tests", "v2")
        };

        var actual = finder.GetDirectories(baseDir, "**/tests/**");

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetDirectories("*", SearchOption.AllDirectories)).MustHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "frobuscator").GetDirectories("tests", SearchOption.TopDirectoryOnly))
         .MustHaveHappened();
        A.CallTo(() =>
             GetFakeDirectory("tools", "metamorphosator").GetDirectories("tests", SearchOption.TopDirectoryOnly))
         .MustHaveHappened();
        A.CallTo(() =>
             GetFakeDirectory("tools", "frobuscator", "tests").GetDirectories("*", SearchOption.AllDirectories))
         .MustHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "metamorphosator", "tests")
            .GetDirectories("*", SearchOption.AllDirectories)).MustHaveHappened();
    }

    [Fact]
    public void GetDirectories_WordWithAsteriskThenGreedyThenWord()
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools");

        A.CallTo(() => baseDir.GetDirectories("meta*", SearchOption.TopDirectoryOnly))
         .Returns(new[] { GetFakeDirectory("tools", "metamorphosator") });

        A.CallTo(() => GetFakeDirectory("tools", "metamorphosator", "tests")
             .GetDirectories("v1", SearchOption.TopDirectoryOnly))
         .Returns(new[]
         {
             GetFakeDirectory("tools", "metamorphosator", "tests", "v1")
         });

        A.CallTo(() =>
             GetFakeDirectory("tools", "metamorphosator").GetDirectories("tests", SearchOption.TopDirectoryOnly))
         .Returns(new[]
         {
             GetFakeDirectory("tools", "metamorphosator", "tests")
         });

        var expected = new[] { GetFakeDirectory("tools", "metamorphosator", "tests", "v1") };
        var actual = finder.GetDirectories(baseDir, "meta*/**/v1");

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetDirectories("meta*", SearchOption.TopDirectoryOnly)).MustHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "frobuscator").GetDirectories(A<string>._, A<SearchOption>._))
         .MustNotHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "metamorphosator").GetDirectories("*", SearchOption.AllDirectories))
         .MustHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "metamorphosator", "tests")
            .GetDirectories("v1", SearchOption.TopDirectoryOnly)).MustHaveHappened();
    }

    [Fact]
    public void GetDirectories_Parent()
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "frobuscator");
        var expected = new[] { GetFakeDirectory("tools") };

        var actual = finder.GetDirectories(baseDir, "../");

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
    }

    [Fact]
    public void GetDirectories_ParentThenParentThenWordThenWord()
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "frobuscator", "tests");

        A.CallTo(() =>
             GetFakeDirectory("tools")
                 .GetDirectories("metamorphosator", SearchOption.TopDirectoryOnly))
         .Returns(new[]
         {
             GetFakeDirectory("tools", "metamorphosator")
         });

        A.CallTo(() =>
             GetFakeDirectory("tools", "metamorphosator")
                 .GetDirectories("addins", SearchOption.TopDirectoryOnly))
         .Returns(new[]
         {
             GetFakeDirectory("tools", "metamorphosator", "addins")
         });

        var expected = new[] { GetFakeDirectory("tools", "metamorphosator", "addins") };

        var actual = finder.GetDirectories(baseDir, "../../metamorphosator/addins");

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.Parent.Parent.GetDirectories("metamorphosator", SearchOption.TopDirectoryOnly))
         .MustHaveHappened();
        A.CallTo(() =>
             GetFakeDirectory("tools", "metamorphosator").GetDirectories("addins", SearchOption.TopDirectoryOnly))
         .MustHaveHappened();
    }

    [Fact]
    public void GetDirectories_StartDirectoryIsNull()
    {
        var finder = new DirectoryFinder();

        Should.Throw<ArgumentNullException>(() => finder.GetDirectories(null, "notused"));
    }

    [Fact]
    public void GetDirectories_PatternIsNull()
    {
        var finder = new DirectoryFinder();

        Should.Throw<ArgumentNullException>(() => finder.GetDirectories(A.Fake<IDirectory>(), null));
    }

    [Fact]
    public void GetDirectories_PatternIsEmpty()
    {
        var directory = A.Fake<IDirectory>();
        var sut = new DirectoryFinder();

        var directories = sut.GetDirectories(directory, string.Empty);

        directories.Count().ShouldBe(1);
        directories.First().ShouldBe(directory);
    }

    [InlineData("tests.*.dll")]
    [InlineData("tests.???.dll")]
    [InlineData("t*.???.dll")]
    [Theory]
    public void GetFiles_WordWithWildcard(string pattern)
    {
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "frobuscator", "tests", "abc");

        A.CallTo(() => baseDir.GetFiles(pattern)).Returns(new[]
        {
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.abc.dll"),
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.123.dll")
        });

        var expected = new[]
        {
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.abc.dll"),
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.123.dll")
        };

        var actual = finder.GetFiles(baseDir, pattern).ToArray();

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.GetFiles(pattern)).MustHaveHappened();
        A.CallTo(() => baseDir.Parent.GetFiles(A<string>._)).MustNotHaveHappened();
    }

    [InlineData("*/tests.*.dll")]
    [InlineData("*/tests.???.dll")]
    [InlineData("*/t*.???.dll")]
    [Theory]
    public void GetFiles_AsteriskThenWordWithWildcard(string pattern)
    {
        var filePattern = pattern.Split('/')[1];
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "frobuscator", "tests");
        var abcDir = GetFakeDirectory("tools", "frobuscator", "tests", "abc");
        var defDir = GetFakeDirectory("tools", "frobuscator", "tests", "def");

        A.CallTo(() => abcDir.GetFiles(filePattern)).Returns(new[]
        {
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.abc.dll"),
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.123.dll")
        });

        A.CallTo(() => defDir.GetFiles(filePattern)).Returns(new[]
            { GetFakeFile("tools", "frobuscator", "tests", "def", "tests.def.dll") });

        var expected = new[]
        {
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.abc.dll"),
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.123.dll"),
            GetFakeFile("tools", "frobuscator", "tests", "def", "tests.def.dll")
        };

        var actual = finder.GetFiles(baseDir, pattern);

        actual.ShouldBe(expected);
        A.CallTo(() => abcDir.GetFiles(filePattern)).MustHaveHappened();
        A.CallTo(() => defDir.GetFiles(filePattern)).MustHaveHappened();
        A.CallTo(() => baseDir.Parent.GetFiles(A<string>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.GetFiles(A<string>._)).MustNotHaveHappened();
        A.CallTo(() => abcDir.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => defDir.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
    }

    [InlineData("**/test*.dll")]
    [InlineData("**/t*.???")]
    [Theory]
    public void GetFiles_GreedyThenWordWithWildcard(string pattern)
    {
        var filePattern = pattern.Split('/')[1];
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools");
        var abcDir = GetFakeDirectory("tools", "frobuscator", "tests", "abc");
        var defDir = GetFakeDirectory("tools", "frobuscator", "tests", "def");
        var v1Dir = GetFakeDirectory("tools", "metamorphosator", "tests", "v1");
        var v2Dir = GetFakeDirectory("tools", "metamorphosator", "tests", "v2");

        A.CallTo(() => abcDir.GetFiles(filePattern)).Returns(new[]
        {
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.abc.dll"),
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.123.dll")
        });

        A.CallTo(() => defDir.GetFiles(filePattern)).Returns(new[]
            { GetFakeFile("tools", "frobuscator", "tests", "def", "tests.def.dll") });

        A.CallTo(() => v1Dir.GetFiles(filePattern)).Returns(new[]
        {
            GetFakeFile("tools", "metamorphosator", "tests", "v1", "test-assembly.dll"),
            GetFakeFile("tools", "metamorphosator", "tests", "v1", "test-assembly.pdb")
        });

        A.CallTo(() => v2Dir.GetFiles(filePattern)).Returns(new[]
        {
            GetFakeFile("tools", "metamorphosator", "tests", "v2", "test-assembly.dll"),
            GetFakeFile("tools", "metamorphosator", "tests", "v2", "test-assembly.pdb")
        });

        var expected = new[]
        {
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.abc.dll"),
            GetFakeFile("tools", "frobuscator", "tests", "abc", "tests.123.dll"),
            GetFakeFile("tools", "frobuscator", "tests", "def", "tests.def.dll"),
            GetFakeFile("tools", "metamorphosator", "tests", "v1", "test-assembly.dll"),
            GetFakeFile("tools", "metamorphosator", "tests", "v1", "test-assembly.pdb"),
            GetFakeFile("tools", "metamorphosator", "tests", "v2", "test-assembly.dll"),
            GetFakeFile("tools", "metamorphosator", "tests", "v2", "test-assembly.pdb")
        };

        var actual = finder.GetFiles(baseDir, pattern);

        actual.ShouldBe(expected);

        foreach (var dir in _fakedDirectories.Values.Where(x => x.FullName != Root))
        {
            A.CallTo(() => dir.GetFiles(filePattern)).MustHaveHappened();
        }

        A.CallTo(() => baseDir.Parent.GetFiles(A<string>._)).MustNotHaveHappened();
        A.CallTo(() => abcDir.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => defDir.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => v1Dir.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => v2Dir.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
    }

    [Fact]
    public void GetFiles_WordThenParentThenWordWithWildcardThenWord()
    {
        var filename = "readme.txt";
        var pattern = "tests/../addin?/" + filename;
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "metamorphosator");
        var targetDir = GetFakeDirectory("tools", "metamorphosator", "addins");

        A.CallTo(() => baseDir.GetDirectories("tests", SearchOption.TopDirectoryOnly)).Returns(new[]
        {
            GetFakeDirectory("tools", "metamorphosator", "tests")
        });

        A.CallTo(() =>
             GetFakeDirectory("tools", "metamorphosator").GetDirectories("addin?", SearchOption.TopDirectoryOnly))
         .Returns(new[] { targetDir });

        A.CallTo(() => targetDir.GetFiles(filename)).Returns(new[]
        {
            GetFakeFile("tools", "metamorphosator", "addins", filename)
        });


        var expected = new[] { GetFakeFile("tools", "metamorphosator", "addins", filename) };

        var actual = finder.GetFiles(baseDir, pattern);

        actual.ShouldBe(expected);
        A.CallTo(() => targetDir.GetFiles(filename)).MustHaveHappened();

        foreach (var dir in _fakedDirectories.Values.Where(x => x != targetDir))
        {
            A.CallTo(() => dir.GetFiles(A<string>._)).MustNotHaveHappened();
        }

        A.CallTo(() => targetDir.GetFiles(filename)).MustHaveHappened();
        A.CallTo(() => targetDir.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
        A.CallTo(() => GetFakeDirectory("tools", "frobuscator").GetDirectories(A<string>._, A<SearchOption>._))
         .MustNotHaveHappened();
    }

    [Fact]
    public void GetFiles_CurrentDirThenAsterisk()
    {
        var pattern = "./*";
        var finder = new DirectoryFinder();
        var baseDir = GetFakeDirectory("tools", "metamorphosator", "addins", "morph");
        var expected = new[]
        {
            GetFakeFile("tools", "metamorphosator", "addins", "morph", "setup.ini"),
            GetFakeFile("tools", "metamorphosator", "addins", "morph", "code.cs")
        };

        A.CallTo(() => baseDir.GetFiles("*")).Returns(expected);

        var actual = finder.GetFiles(baseDir, pattern);

        actual.ShouldBe(expected);
        A.CallTo(() => baseDir.GetFiles("*")).MustHaveHappened();
        A.CallTo(() => baseDir.Parent.GetFiles(A<string>._)).MustNotHaveHappened();
        A.CallTo(() => baseDir.Parent.GetDirectories(A<string>._, A<SearchOption>._)).MustNotHaveHappened();
    }

    [Fact]
    public void GetFiles_StartDirectoryIsNull()
    {
        var finder = new DirectoryFinder();

        Should.Throw<ArgumentNullException>(() => finder.GetFiles(null, "notused").ToArray());
    }

    [Fact]
    public void GetFiles_PatternIsNull()
    {
        var finder = new DirectoryFinder();

        Should.Throw<ArgumentNullException>(() => finder.GetDirectories(A.Fake<IDirectory>(), null));
    }

    [Fact]
    public void GetFiles_PatternIsEmpty()
    {
        var finder = new DirectoryFinder();

        Should.Throw<ArgumentException>(() => finder.GetFiles(A.Fake<IDirectory>(), string.Empty).ToArray());
    }

    private static string CreateAbsolutePath(IEnumerable<string> parts)
    {
        return CreateAbsolutePath(parts.ToArray());
    }

    private static string CreateAbsolutePath(params string[] parts)
    {
        var relativePath = CombinePath(parts);
        return Path.DirectorySeparatorChar == '\\' ? "c:\\" + relativePath : "/" + relativePath;
    }

    private IDirectory GetFakeDirectory(params string[] parts)
    {
        return _fakedDirectories[CreateAbsolutePath(parts)];
    }

    private IFile GetFakeFile(params string[] parts)
    {
        return _fakedFiles[CreateAbsolutePath(parts)];
    }

    private static string CombinePath(params string[] parts)
    {
        return parts.Aggregate(Path.Combine);
    }
}
