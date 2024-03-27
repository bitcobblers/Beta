using System.Reflection;
using Beta.Internal;
using Beta.Sdk.Interfaces;
using Xunit.Abstractions;

namespace Beta.Tests.Internal;

public class BetaEngineControllerTests
{
    public class ExecuteIfInitializedMethod(ITestOutputHelper output) : BetaEngineControllerTests
    {
        private readonly ILogger _logger = new XUnitLogger(output);

        [Fact]
        public void ReturnsFunctionResultIfInitialized()
        {
            // Arrange.
            var controller =
                new BetaEngineController(
                    true,
                    _logger,
                    A.Dummy<Assembly>(),
                    A.Fake<ITestAssemblyExplorer>());

            // Act.
            var result = controller.ExecuteIfInitialized(false, () => true);

            // Assert.
            result.ShouldBeTrue();
        }

        [Fact]
        public void ReturnsDefaultValueIfNotInitialized()
        {
            // Arrange.
            var controller =
                new BetaEngineController(
                    false,
                    _logger,
                    A.Dummy<Assembly>(),
                    A.Fake<ITestAssemblyExplorer>());

            // Act.
            var result = controller.ExecuteIfInitialized(true, () => false);

            // Assert.
            result.ShouldBeTrue();
        }

        [Fact]
        public void ReturnsDefaultValueIfExceptionIsThrown()
        {
            // Arrange.
            var controller =
                new BetaEngineController(
                    true,
                    _logger,
                    A.Dummy<Assembly>(),
                    A.Fake<ITestAssemblyExplorer>());

            // Act.
            var result = controller.ExecuteIfInitialized(
                true,
                () => throw new Exception("I failed"));

            // Assert.
            result.ShouldBeTrue();
        }
    }
}
