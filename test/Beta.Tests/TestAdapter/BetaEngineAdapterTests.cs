using System.Reflection;
using System.Runtime.Loader;
using Beta.TestAdapter;
using mock_assembly;
using Xunit.Abstractions;

namespace Beta.Tests.TestAdapter;

public class BetaEngineAdapterTests
{
    public class GetControllerMethod(ITestOutputHelper output) : BetaEngineAdapterTests
    {
        private readonly ITestLogger _logger = new XUnitTestLogger(output);

        [Fact]
        public void CanGetController()
        {
            // Arrange.
            var assemblyPath = typeof(SampleTests).Assembly.Location;
            var adapter = new BetaEngineAdapter(assemblyPath, _logger);

            // Act.
            var controller = adapter.GetController();

            // Assert.
            controller.ShouldNotBeNull();
        }

        [Fact]
        public void MissingTestAssemblyReturnsNull()
        {
            // Arrange.
            var adapter = new FakeBetaEngineAdapter(
                false,
                true,
                A.Dummy<object>(),
                _logger);

            // Act.
            var result = adapter.GetController();

            // Assert.
            result.ShouldBeNull();
        }

        [Fact]
        public void MissingBetaAssemblyReturnsNull()
        {
            // Arrange.
            var adapter = new FakeBetaEngineAdapter(
                true,
                false,
                A.Dummy<object>(),
                _logger);

            // Act.
            var result = adapter.GetController();

            // Assert.
            result.ShouldBeNull();
        }

        private class FakeBetaEngineAdapter(
            bool hasTestAssembly,
            bool hasBetaAssembly,
            object? controllerInstance,
            ITestLogger logger)
            : BetaEngineAdapter(
                "assembly-path.dll",
                logger,
                _ => A.Fake<AssemblyLoadContext>())
        {
            /// <inheritdoc />
            protected override Assembly? LoadBetaAssembly(Assembly assembly) =>
                hasBetaAssembly
                    ? A.Dummy<Assembly>()
                    : null;

            /// <inheritdoc />
            protected override Assembly? LoadTestAssembly() =>
                hasTestAssembly
                    ? A.Dummy<Assembly>()
                    : null;

            /// <inheritdoc />
            protected override object? CreateController(string typeName, Assembly betaAssembly, object[] args) =>
                controllerInstance;
        }
    }

    public class MaybeThrowsMethod : BetaEngineAdapterTests
    {
        [Fact]
        public void ThrownExceptionIsTrappedAndLogged()
        {
            // Arrange.
            var logger = A.Fake<ITestLogger>();

            // Act.
            _ = BetaEngineAdapter.MaybeThrows(
                logger,
                "ignored",
                () =>
                {
                    throw new Exception("expected");
#pragma warning disable CS0162 // Unreachable code detected
                    return "ignored";
#pragma warning restore CS0162 // Unreachable code detected
                },
                "ignored {0}");

            // Assert.
            A.CallTo(() => logger.Error(A<string>._, A<Exception>._)).MustHaveHappened();
        }

        [Fact]
        public void ThrownExceptionIsTrappedAndReturnsNull()
        {
            // Arrange.
            var logger = A.Fake<ITestLogger>();

            // Act.
            var result = BetaEngineAdapter.MaybeThrows(
                logger,
                "ignored",
                () =>
                {
                    throw new Exception("expected");
#pragma warning disable CS0162 // Unreachable code detected
                    return "ignored";
#pragma warning restore CS0162 // Unreachable code detected
                },
                "ignored {0}");

            // Assert.
            result.ShouldBeNull();
        }
    }
}
