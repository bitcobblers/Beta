using Beta.Shouldly.ShouldBe;
using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using static Beta.CommonSteps;

namespace Beta.Tests.Demos;

[PublicAPI]
public class CalculatorDemo : TestContainer
{
    private static IEnumerable<Input> AdditionInput =>
    [
        new Input(1, 2, 3),
        new Input(2, 3, 5)
    ];

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<Calculator>();
    }

    //[Beta]
    //public BetaTest AddTest1()
    //{
    //    return Test(() =>
    //        from calculator in Require<Calculator>()
    //        let result = Apply(() => calculator.Add(1, 2))
    //        select result.ShouldBe(3));
    //}

    [Beta]
    public BetaTest AddTestMany()
    {
        return Test(AdditionInput, i =>
            from calculator in Gather(() => Task.FromResult(new Calculator()))
            let result = Apply(async () => (await calculator).Add(i.A, i.B))
            select result.ShouldBe(i.Expected));
    }

    [TestCase(1, 2, 3)]
    [TestCase(2, 3, 5)]
    public void AddTest(int a, int b, int expected)
    {
        var calculator = new Calculator();
        var result = calculator.Add(a, b);
        result.ShouldBe(expected);
    }

    // ---

    private record Input(int A, int B, int Expected);

    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
