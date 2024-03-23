namespace Beta.Tests;

public class StepChainerTests
{
    [Fact]
    public void SelectWithStepReturnsTestResult()
    {
        // Arrange
        var step = new Step<int>(() => 42);

        // Act
        var result = step.Select(x => x.ToString());

        // Assert
        result.Value.ShouldBe("42");
    }

    [Fact]
    public void SelectWithTestResultReturnsResult()
    {
        // Arrange
        var result = new TestResult<int>(42);

        // Act
        var newResult = result.Select(x => x.ToString());

        // Assert
        newResult.ShouldBe("42");
    }

    [Fact]
    public void SelectManyWithStepsReturnsStep()
    {
        // Arrange
        var step = new Step<int>(() => 42);

        // Act
        var result = step.SelectMany(
            _ => new Step<string>(() => "Hello"),
            (x, y) => $"{x} {y}");

        // Assert
        result.Evaluate().ShouldBe("42 Hello");
    }
}
