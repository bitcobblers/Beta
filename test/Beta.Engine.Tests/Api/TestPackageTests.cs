using Beta.Engine.Api;

namespace Beta.Engine.Tests.Api;

public class TestPackageTests
{
    public class SingleAssembly : TestPackageTests
    {
        private readonly TestPackage package = new("test.dll");

        [Fact]
        public void PackageIDsAreUnique()
        {
            var another = new TestPackage("another.dll");
            another.Id.ShouldNotBe(package.Id);
        }

        [Fact]
        public void AssemblyPathIsUsedAsFilePath()
        {
            // Assert.That(package.FullName, Is.EqualTo(Path.GetFullPath("test.dll")));
            Path.GetFullPath("test.dll").ShouldBe(package.FullName);
        }

        [Fact]
        public void FileNameIsUsedAsPackageName()
        {
            //Assert.That(package.Name, Is.EqualTo("test.dll"));
            package.Name.ShouldBe("test.dll");
        }

        [Fact]
        public void HasNoSubPackages()
        {
            //Assert.That(package.SubPackages.Count, Is.EqualTo(0));
            package.SubPackages.Count.ShouldBe(0);
        }
    }

    public class MultipleAssemblies
    {
        private readonly TestPackage package = new(new[] { "test1.dll", "test2.dll", "test3.dll" });

        [Fact]
        public void PackageIsAnonymous()
        {
            package.FullName.ShouldBeNull();
        }

        [Fact]
        public void PackageContainsThreeSubpackages()
        {
            package.SubPackages.Count.ShouldBe(3);
            package.SubPackages[0].FullName.ShouldBe(Path.GetFullPath("test1.dll"));
            package.SubPackages[1].FullName.ShouldBe(Path.GetFullPath("test2.dll"));
            package.SubPackages[2].FullName.ShouldBe(Path.GetFullPath("test3.dll"));
        }
    }
}
