using System.Runtime.InteropServices;
using Beta.Engine.Internal;

namespace Beta.Engine.Core.Tests.Internal;

[Collection("Serial")]
public class PathUtilsTests : PathUtils
{
    private static void AssertSamePathOrUnder(string path1, string path2)
    {
        var msg = "\r\n\texpected: Same path or under <{0}>\r\n\t but was: <{1}>";
        SamePathOrUnder(path1, path2).ShouldBeTrue(msg);
    }

    private static void AssertNotSamePathOrUnder(string path1, string path2)
    {
        var msg = "\r\n\texpected: Not same path or under <{0}>\r\n\t but was: <{1}>";
        SamePathOrUnder(path1, path2).ShouldBeFalse(msg);
    }

    public class Windows : PathUtilsTests, IDisposable
    {
        public Windows()
        {
            DirectorySeparatorChar = '\\';
            AltDirectorySeparatorChar = '/';
        }

#pragma warning disable CA1816
        public void Dispose()
#pragma warning restore CA1816
        {
            DirectorySeparatorChar = Path.DirectorySeparatorChar;
            AltDirectorySeparatorChar = Path.AltDirectorySeparatorChar;
        }

        [InlineData("c:\\", true)]
        [InlineData("c:\\foo\\bar\\", true)]
        [InlineData("c:/foo/bar/", true)]
        [InlineData("c:\\foo\\bar", true)]
        [InlineData("c:/foo/bar", true)]
        [InlineData("c:bar\\", false)]
        [InlineData("c:bar/", false)]
        [InlineData("c:bar", false)]
        [InlineData("ä:\\bar", false)]
        [InlineData("ä://bar", false)]
        [InlineData("\\\\server01\\foo", true)]
        [InlineData("\\server01\\foo", false)]
        [InlineData("c:", false)]
        [InlineData("/foo/bar", false)]
        [InlineData("/", false)]
        [InlineData("\\a\\b", false)]
        [Theory]
        public void IsFullyQualifiedPath(string path, bool expected)
        {
            IsFullyQualifiedWindowsPath(path).ShouldBe(expected);
        }

        [Fact]
        public void NullPathThrowsArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => IsFullyQualifiedWindowsPath(null));
        }

        [InlineData(@"C:\folder1\.\folder2\..\file.tmp", @"C:\folder1\file.tmp")]
        [InlineData(@"folder1\.\folder2\..\file.tmp", @"folder1\file.tmp")]
        [InlineData(@"folder1\folder2\.\..\file.tmp", @"folder1\file.tmp")]
        [InlineData(@"folder1\folder2\..\.\..\file.tmp", @"file.tmp")]
        [InlineData(@"folder1\folder2\..\..\..\file.tmp", @"file.tmp")]
        [Theory]
        public void CanonicalizeScenarios(string given, string expected)
        {
            Canonicalize(given).ShouldBe(expected);
        }

        [InlineData(@"c:\folder1", @"c:\folder1\folder2\folder3", @"folder2\folder3")]
        [InlineData(@"c:\folder1", @"c:\folder2\folder3", @"..\folder2\folder3")]
        [InlineData(@"c:\folder1", @"bin\debug", @"bin\debug")]
        [InlineData(@"C:\folder", @"D:\folder", null)]
        [InlineData(@"C:\", @"D:\", null)]
        [InlineData(@"C:", @"D:", null)]
        [InlineData(@"C:\folder1", @"C:\folder1", @"")]
        [InlineData(@"C:\", @"C:\", @"")]

        // First filePath consisting just of a root:
        [InlineData(@"C:\", @"C:\folder1\folder2", @"folder1\folder2")]

        // Trailing directory separator in first filePath shall be ignored:
        [InlineData(@"c:\folder1\", @"c:\folder1\folder2\folder3", @"folder2\folder3")]

        // Case-insensitive behavior, preserving 2nd filePath directories in result:
        [InlineData(@"c:\folder1", @"c:\folder1\Folder2\Folder3", @"Folder2\Folder3")]
        [InlineData(@"c:\folder1", @"C:\Folder2\folder3", @"..\Folder2\folder3")]
        [Theory]
        public void RelativePathScenarios(string path1, string path2, string? expected)
        {
            var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            windows.ShouldBeTrue();
            RelativePath(path1, path2).ShouldBe(expected);
        }

        public class SamePathOrUnderScenarios : Windows
        {
            [InlineData(@"C:\folder1\folder2\folder3", @"C:\folder1\.\folder2\junk\..\folder3")]
            [InlineData(@"C:\folder1\folder2\", @"C:\folder1\.\folder2\junk\..\folder3")]
            [InlineData(@"C:\folder1\folder2", @"C:\folder1\.\folder2\junk\..\folder3")]
            [InlineData(@"C:\", @"C:\")]
            [InlineData(@"C:\", @"C:\bin\debug")]
            [Theory]
            public void Same(string path1, string path2)
            {
                AssertSamePathOrUnder(path1, path2);
            }

            [InlineData(@"C:\folder1\folder2", @"C:\folder1\.\folder22\junk\..\folder3")]
            [InlineData(@"C:\folder1\folder2ile.tmp", @"D:\folder1\.\folder2\folder3\file.tmp")]
            [InlineData(@"C:\", @"D:\")]
            [Theory]
            public void NotSame(string path1, string path2)
            {
                AssertNotSamePathOrUnder(path1, path2);
            }
        }
    }

    public class Unix : PathUtilsTests, IDisposable
    {
        public Unix()
        {
            DirectorySeparatorChar = '/';
            AltDirectorySeparatorChar = '\\';
        }

#pragma warning disable CA1816
        public void Dispose()
#pragma warning restore CA1816
        {
            DirectorySeparatorChar = Path.DirectorySeparatorChar;
            AltDirectorySeparatorChar = Path.AltDirectorySeparatorChar;
        }

        [InlineData("/foo/bar", true)]
        [InlineData("/", true)]
        [InlineData("/z", true)]
        [InlineData("c:\\foo\\bar\\", false)]
        [InlineData("c:/foo/bar/", false)]
        [InlineData("c:\\foo\\bar", false)]
        [InlineData("c:/foo/bar", false)]
        [InlineData("c:bar\\", false)]
        [InlineData("c:bar/", false)]
        [InlineData("c:bar", false)]
        [InlineData("ä:\\bar", false)]
        [InlineData("ä://bar", false)]
        [InlineData("\\\\server01\\foo", false)]
        [InlineData("\\server01\\foo", false)]
        [InlineData("c:", false)]
        [InlineData("\\a\\b", false)]
        [Theory]
        public void IsFullyQualifiedPath(string path, bool expected)
        {
            IsFullyQualifiedUnixPath(path).ShouldBe(expected);
        }

        [Fact]
        public void NullPathThrowsArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => IsFullyQualifiedUnixPath(null));
        }

        [InlineData("/folder1/./folder2/../file.tmp", "/folder1/file.tmp")]
        [InlineData("folder1/./folder2/../file.tmp", "folder1/file.tmp")]
        [InlineData("folder1/folder2/./../file.tmp", "folder1/file.tmp")]
        [InlineData("folder1/folder2/.././../file.tmp", "file.tmp")]
        [InlineData("folder1/folder2/../../../file.tmp", "file.tmp")]
        [Theory]
        public void CanonicalizeScenarios(string given, string expected)
        {
            Canonicalize(given).ShouldBe(expected);
        }

        [InlineData("/folder1", "/folder1/folder2/folder3", "folder2/folder3")]
        [InlineData("/folder1", "/folder2/folder3", "../folder2/folder3")]
        [InlineData("/folder1", "bin/debug", "bin/debug")]
        [InlineData("/folder", "/other/folder", "../other/folder")]
        [InlineData("/a/b/c", "/a/d", "../../d")]
        [InlineData("/a/b", "/a/b", "")]
        [InlineData("/", "/", "")]

        // First filePath consisting just of a root:
        [InlineData("/", "/folder1/folder2", "folder1/folder2")]

        // Trailing directory separator in first filePath shall be ignored:
        [InlineData("/folder1/", "/folder1/folder2/folder3", "folder2/folder3")]

        // Case-sensitive behavior:
        [InlineData("/folder1", "/Folder1/Folder2/folder3", "../Folder1/Folder2/folder3")]
        [Theory]
        public void RelativePathScenarios(string path1, string path2, string expected)
        {
            RelativePath(path1, path2).ShouldBe(expected);
        }

        public class SamePathOrUnderScenarios : Unix
        {
            [InlineData("/folder1/folder2/folder3", "/folder1/./folder2/junk/../folder3")]
            [InlineData("/folder1/folder2/", "/folder1/./folder2/junk/../folder3")]
            [InlineData("/folder1/folder2", "/folder1/./folder2/junk/../folder3")]
            [InlineData("/", "/")]
            [InlineData("/", "/bin/debug")]
            [Theory]
            public void Same(string path1, string path2)
            {
                AssertSamePathOrUnder(path1, path2);
            }

            [InlineData("/folder1/folder2", "/folder1/./Folder2/junk/../folder3")]
            [InlineData("/folder1/folder2", "/folder1/./folder22/junk/../folder3")]
            [Theory]
            public void NotSame(string path1, string path2)
            {
                AssertNotSamePathOrUnder(path1, path2);
            }
        }
    }
}
