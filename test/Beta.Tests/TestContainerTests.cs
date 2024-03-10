using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Beta.Tests;

public class TestContainerTests
{
    public class DiscoverMethod : TestContainerTests
    {
        [Fact]
        public void ReturnsSingleTest()
        {
            // Arrange
            var container = new StubWithTest();

            // Act
            var tests = container.Discover().ToArray();

            // Assert
            tests.ShouldHaveSingleItem();
        }

        [Fact]
        public void ReturnsNoTests()
        {
            // Arrange
            var container = new StubWithNoTests();

            // Act
            var tests = container.Discover().ToArray();

            // Assert
            tests.ShouldBeEmpty();
        }

        private class StubWithTest : TestContainer
        {
            public BetaTest SingleTest()
            {
                return Test(() => new Proof<int>(42));
            }
        }

        private class StubWithNoTests : TestContainer
        {
            [UsedImplicitly]
            [SuppressMessage("Performance", "CA1822:Mark members as static")]
            public void NotATest()
            {
            }
        }
    }

    public class PrepareMethod : TestContainerTests
    {
        [Fact]
        public void DefinitivelyAssignsServicesProvider()
        {
            // Arrange
            var container = new TestContainer();

            // Act
            container.Prepare();

            // Assert
            container.ServicesProvider.ShouldNotBeNull();
        }

        [Fact]
        public void RegistersServices()
        {
            // Arrange
            var container = new StubThatRegistersService();

            // Act
            container.Prepare();

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
            container.Prepare();

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
            container.Prepare();

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

            public new Step<T> Require<T>() where T : notnull
            {
                return base.Require<T>();
            }

            public new Step<object> Require(Type type)
            {
                return base.Require(type);
            }
        }
    }
}
