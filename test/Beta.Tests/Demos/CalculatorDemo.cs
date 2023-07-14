using Beta.AAA;
using FluentAssertions;

namespace Beta.Tests.Demos;

public class CalculatorDemo : BetaTest
{
    public CalculatorDemo()
    {
        AdditionInput.ToTest(this, "Addition Tests", (test, input) =>
        {
            test.AddSteps(steps => steps
                .Arrange(() => new Calculator())
                .Act(calc => calc.Add(input.A, input.B))
                .Assert(result => result.Should().Be(input.Expected))
            );
        });
    }

    public IEnumerable<Input> AdditionInput
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

    public record Input(int A, int B, int Expected);

    private class Calculator
    {
        public int Add(int a, int b) => a + b;
    }
}
