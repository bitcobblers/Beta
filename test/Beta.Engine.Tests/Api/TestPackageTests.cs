namespace Beta.Engine.Tests.Api;

public class TestPackageTests
{
    public class SingleAssembly : TestPackageTests
    {
        private readonly TestPackage _package = new("test.dll");

        [Fact]
        public void PackageIDsAreUnique()
        {
            var another = new TestPackage("another.dll");
            another.Id.ShouldNotBe(_package.Id);
        }

        [Fact]
        public void AssemblyPathIsUsedAsFilePath()
        {
            Path.GetFullPath("test.dll").ShouldBe(_package.FullName);
        }

        [Fact]
        public void FileNameIsUsedAsPackageName()
        {
            _package.Name.ShouldBe("test.dll");
        }

        [Fact]
        public void HasNoSubPackages()
        {
            _package.SubPackages.Count.ShouldBe(0);
        }
    }

    public class MultipleAssemblies
    {
        private readonly TestPackage _package = new(["test1.dll", "test2.dll", "test3.dll"]);

        [Fact]
        public void PackageIsAnonymous()
        {
            _package.FullName.ShouldBeNull();
        }

        [Fact]
        public void PackageContainsThreeSubpackages()
        {
            _package.SubPackages.Count.ShouldBe(3);
            _package.SubPackages[0].FullName.ShouldBe(Path.GetFullPath("test1.dll"));
            _package.SubPackages[1].FullName.ShouldBe(Path.GetFullPath("test2.dll"));
            _package.SubPackages[2].FullName.ShouldBe(Path.GetFullPath("test3.dll"));
        }
    }
}
