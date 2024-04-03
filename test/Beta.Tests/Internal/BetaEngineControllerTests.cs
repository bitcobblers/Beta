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
                    _logger,
                    A.Dummy<Assembly>(),
                    A.Fake<ITestAssemblyExplorer>(),
                    A.Fake<ITestRunner>());

            // Act.
            var result = controller.Execute(false, () => true);

            // Assert.
            result.ShouldBeTrue();
        }

        [Fact]
        public void ReturnsDefaultValueIfExceptionIsThrown()
        {
            // Arrange.
            var controller =
                new BetaEngineController(
                    _logger,
                    A.Dummy<Assembly>(),
                    A.Fake<ITestAssemblyExplorer>(),
                    A.Fake<ITestRunner>());

            // Act.
            var result = controller.Execute(
                true,
                () => throw new Exception("I failed"));

            // Assert.
            result.ShouldBeTrue();
        }
    }
}
