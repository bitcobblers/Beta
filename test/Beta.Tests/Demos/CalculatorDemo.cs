using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using static Beta.CommonSteps.AAA;

namespace Beta.Tests.Demos;

public class CalculatorDemo : TestContainer
{
    // ReSharper disable once MemberCanBeMadeStatic.Global
    public BetaTest AddTest()
    {
        return NewTest(AdditionInput)
            .Apply(i =>
                from calculator in Arrange(GetCalculator)
                select Act(() => calculator.Add(i.A, i.B)))
            .Proof(_ => { });

        // return NewTest(() =>
        //         from calculator in Arrange(GetCalculator)
        //         select Act(() => calculator.Add(1, 1)))
        //     .Proof(_ => { });
    }

    private static Calculator GetCalculator() => new();

    private static int GetValue(int value) => value;

    // ---

    private record Input(int A, int B, int Expected);

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

    public class Calculator
    {
        public int Add(int a, int b) => a + b;
    }
}