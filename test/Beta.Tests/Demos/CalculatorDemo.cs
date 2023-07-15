using System.Linq.Expressions;
using FluentAssertions;

namespace Beta.Tests.Demos;

public class CalculatorDemo : BetaTest
{
    public CalculatorDemo()
    {
        // Guided test.
        //AdditionInput.ToTest(this, "Addition Tests (Guided)", (test, input) =>
        //{
        //    test
        //        .AddSteps(steps => steps
        //        .Arrange(() => new Calculator())
        //        .Act(calc => calc.Add(input.A, input.B))
        //        .Assert(result =>
        //        {
        //            result.Should().Be(input.Expected);
        //        })
        //    );
        //});

        //// Basic (escape hatch) test.
        //AdditionInput.ToTest(this, "Addition Tests (Basic)", (test, input) =>
        //{
        //    test
        //        .Basic(() =>
        //        {
        //            // Arrange.
        //            var calculator = new Calculator();

        //            // Act.
        //            int result = calculator.Add(input.A, input.B);

        //            // Assert.
        //            result.Should().Be(input.Expected);
        //        });
        //});

        UnitTest("Addition Tests (classic)", AdditionInput, (_, input) =>
        {
            // Arrange.
            var calculator = new Calculator();

            // Act.
            int result = calculator.Add(input.A, input.B);

            // Assert.
            result.Should().Be(input.Expected);
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
