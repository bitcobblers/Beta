// ReSharper disable UnusedType.Local
// ReSharper disable UnusedMember.Local

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
        public void ReturnsMultipleTests()
        {
            // Arrange
            var container = new StubWithEnumerableTests();

            // Act
            var tests = container.Discover().ToArray();

            // Assert
            tests.Length.ShouldBe(2);
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
                return Test(new Proof<int>(42));
            }
        }

        private class StubWithEnumerableTests : TestContainer
        {
            public IEnumerable<BetaTest> MultipleTests()
            {
                return Test(
                    new EnumerableScenarioSource<int>([1, 2]),
                    x => new Proof<int>(x));
            }
        }

        private class StubWithNoTests : TestContainer
        {
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

    public class GatherMethod : TestContainerTests
    {
        [Fact]
        public void CanGatherFromValue()
        {
            // Arrange
            var container = new StubContainer();

            // Act
            var step = container.Gather(42);

            // Assert
            step.Evaluate().ShouldBe(42);
        }

        [Fact]
        public void CanGatherFromHandler()
        {
            // Arrange
            var container = new StubContainer();

            // Act
            var step = container.Gather(() => 42);

            // Assert
            step.Evaluate().ShouldBe(42);
        }

        private class StubContainer : TestContainer
        {
            public new Step<T> Gather<T>(T value)
            {
                return base.Gather(value);
            }

            public new Step<T> Gather<T>(Func<T> handler)
            {
                return base.Gather(handler);
            }
        }
    }

    public class ApplyMethod : TestContainerTests
    {
        [Fact]
        public async Task CanApplyFromHandler()
        {
            // Arrange
            var container = new StubContainer();

            // Act
            var proof = container.Apply(() => 42);

            // Assert
            (await proof.Actual).ShouldBe(42);
        }

        [Fact]
        public async Task CanApplyFromAsyncHandler()
        {
            // Arrange
            var container = new StubContainer();

            // Act
            var proof = container.Apply(() => Task.FromResult(42));

            // Assert
            (await proof.Actual).ShouldBe(42);
        }

        private class StubContainer : TestContainer
        {
            public new Proof<T> Apply<T>(Func<T> handler)
            {
                return base.Apply(handler);
            }

            public new Proof<T> Apply<T>(Func<Task<T>> handler)
            {
                return base.Apply(handler);
            }
        }
    }
}