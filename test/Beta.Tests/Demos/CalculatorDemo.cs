using FluentAssertions;

namespace Beta.Tests.Demos;

public class CalculatorDemo : BetaTest
{
    public CalculatorDemo()
    {
        BasicTest("Addition Tests (basic)", AdditionInput, (_, input) =>
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            int result = calculator.Add(input.A, input.B);

            // Assert.
            result.Should().Be(input.Expected);
        });

        GuidedTest("Addition Tests (guided)", AdditionInput, (test, input) =>
        {
            test
                .Arrange(() => new Calculator())
                .Act(calc => calc.Add(input.A, input.B))
                .Assert(result =>
                {
                    result.Should().Be(input.Expected);
                });
        });
    }

    private static IEnumerable<Input> AdditionInput
    {
        get
        {
            return new Input[]
            {
                new(1, 2, 3),
                new(2, 3, 5)
            };
        }
    }

    private record Input(int A, int B, int Expected);

    public class Calculator
    {
        public int Add(int a, int b) => a + b;
    }
}