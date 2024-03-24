using Beta.TestAdapter;

namespace Beta.Tests.TestAdapter;

public class NavigationDataProviderTests
{
    [Fact]
    public void CanGetNavigationDataForThisMethod()
    {
        // Arrange.
        var assemblyPath = GetType().Assembly.Location;
        var className = GetType().FullName!;
        using var provider = new NavigationDataProvider(assemblyPath);

        // Act.
        var result = provider.Get(className, nameof(CanGetNavigationDataForThisMethod));

        // Assert.
        result.ShouldNotBeNull();
        result.FileName.ShouldNotBeNullOrWhiteSpace();
        result.LineNumber.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void GettingNavigationDataForUnknownMethodReturnsNull()
    {
        // Arrange.
        var assemblyPath = GetType().Assembly.Location;
        var className = GetType().FullName!;
        using var provider = new NavigationDataProvider(assemblyPath);

        // Act.
        var result = provider.Get(className, "unknown_method");

        // Assert.
        result.ShouldBeNull();
    }

    [Fact]
    public void GettingNavigationDataForUnknownClassReturnsNull()
    {
        // Arrange.
        var assemblyPath = GetType().Assembly.Location;
        using var provider = new NavigationDataProvider(assemblyPath);

        // Act.
        var result = provider.Get("unknown_class", nameof(GettingNavigationDataForUnknownClassReturnsNull));

        // Assert.
        result.ShouldBeNull();
    }

    [Fact]
    public void GettingNavigationDataForUnknownAssemblyReturnsNull()
    {
        // Arrange.
        using var provider = new NavigationDataProvider("unknown_assembly.dll");

        // Act.
        var result = provider.Get("ignored", "ignored");

        // Assert.
        result.ShouldBeNull();
    }
}
