using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using static Beta.CommonSteps.AAA;


namespace Beta.Tests.Demos;

public class CalculatorDemo : BetaTest
{
    // ReSharper disable once MemberCanBeMadeStatic.Global
    public Proof AddTest()
    {
        var apply = (Input i) =>
            from calculator in Arrange(GetCalculator)
            select Act(() => calculator.Add(i.A, i.B));

        return Proof.New(
            AdditionInput,
            apply,
            Axiom.From(apply));
    }

    private static Calculator GetCalculator() => new();

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