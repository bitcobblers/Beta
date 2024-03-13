using System.Text;
using Beta.Engine.Internal;

namespace Beta.Engine.Core.Tests.Internal;

/// <summary>
///     Tests the implementation of <see cref="AddinsFileReader" />.
/// </summary>
public class AddinsFileReaderTests
{
    [Fact]
    public void Read_IFile_Null()
    {
        var reader = new AddinsFileReader();

        Should.Throw<ArgumentNullException>(() => reader.Read(null));
    }

    [Fact]
    public void Read_Stream()
    {
        // Arrange.
        var input = string.Join(Environment.NewLine,
            "# This line is a comment and is ignored. The next (blank) line is ignored as well.", "",
            "*.dll                   # include all dlls in the same directory",
            "addins/*.dll            # include all dlls in the addins directory too",
            "special/myassembly.dll  # include a specific dll in a special directory",
            "some/other/directory/  # process another directory, which may contain its own addins file",
            "# note that an absolute path is allowed, but is probably not a good idea in most cases",
            "/unix/absolute/directory");

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        // Act.
        var result = AddinsFileReader.Read(stream).ToArray();

        // Assert.
        result.Length.ShouldBe(5);
        result.ShouldContain("*.dll");
        result.ShouldContain("addins/*.dll");
        result.ShouldContain("special/myassembly.dll");
        result.ShouldContain("some/other/directory/");
        result.ShouldContain("/unix/absolute/directory");
    }

    [Fact]
    // [Platform("win")]
    public void Read_Stream_TransformBackslash_Windows()
    {
        // Arrange.
        var input = string.Join(Environment.NewLine, @"c:\windows\absolute\directory");
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        // Act.
        var result = AddinsFileReader.Read(stream).ToArray();

        // Assert.
        result.Length.ShouldBe(1);
        result.ShouldContain("c:/windows/absolute/directory");
    }

    // [Test]
    // [Platform("linux,macosx,unix")]
    // public void Read_Stream_TransformBackslash_NonWindows()
    // {
    //     IEnumerable<string> result;
    //
    //     using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("this/is/a\\ path\\ with\\ spaces/")))
    //     {
    //         // Act
    //         result = AddinsFileReader.Read(stream);
    //     }
    //
    //     Assert.That(result, Has.Count.EqualTo(1));
    //     Assert.That(result, Contains.Item("this/is/a\\ path\\ with\\ spaces/"));
    // }
}
