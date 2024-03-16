using Microsoft.Extensions.DependencyInjection;

namespace Beta.Tests;

public class TestContainerTests
{
    public class PrepareMethod : TestContainerTests
    {
        [Fact]
        public void DefinitivelyAssignsServicesProvider()
        {
            // Arrange
            var container = new TestContainer();

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

        private class StubThatRegistersService : TestContainer
        {
            protected override void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton<StubType>();
            }
        }
    }

    public class RequireMethod : TestContainerTests
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

        private class StubContainer : TestContainer
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