using Microsoft.Extensions.DependencyInjection;

namespace Beta.Tests;

public class TestSuiteTests
{
    public class PrepareMethod : TestSuiteTests
    {
        [Fact]
        public void DefinitivelyAssignsServicesProvider()
        {
            // Arrange
            var container = new TestSuite.DI();

            // Act
            container.Initialize();

            // Assert
            container.ServicesProvider.ShouldNotBeNull();
        }

        [Fact]
        public void RegistersServices()
        {
            // Arrange
            var container = new StubThatRegistersService();

            // Act
            container.Initialize();

            // Assert
            container.ServicesProvider!.GetService<StubType>().ShouldNotBeNull();
        }

        private class StubType;

        private class StubThatRegistersService : TestSuite.DI
        {
            protected override void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton<StubType>();
            }
        }
    }

    public class RequireMethod : TestSuiteTests
    {
        [Fact]
        public void CanResolveGenericType()
        {
            // Arrange
            var container = new StubContainer();
            container.Initialize();

            // Act
            var step = container.Require<StubType>();

            // Assert
            step.Evaluate().ShouldNotBeNull();
        }

        [Fact]
        public void CanResolveType()
        {
            // Arrange
            var container = new StubContainer();
            container.Initialize();

            // Act
            var step = container.Require(typeof(StubType));

            // Assert
            step.Evaluate().ShouldNotBeNull();
        }

        private class StubType;

        private class StubContainer : TestSuite.DI
        {
            protected override void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton<StubType>();
            }

            public new Step<T> Require<T>() where T : notnull => base.Require<T>();

            public new Step<object> Require(Type type) => base.Require(type);
        }
    }
}
