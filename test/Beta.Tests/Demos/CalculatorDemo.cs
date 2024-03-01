using Beta.Shouldly;
using Beta.Shouldly.ShouldBe;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Beta.Tests.Demos;

public class CalculatorDemo : TestContainer
{
    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<Calculator>();
    }

    [PublicAPI]
    public BetaTest AddTestNoInput()
    {
        return Test(from a in Gather(() => 1)
                    let r = Apply(() => a == 1)
                    select r.ShouldBeTrue());
    }

    [PublicAPI]
    public BetaTest AddTestWithInput()
    {
        return Test(AdditionInput, i =>
            from calculator in Require<Calculator>()
            from a in Gather(() => 1)
            from b in Gather(2)
            let c = Apply(() => calculator.Add(a, b))
            select c.ShouldBe(i.Expected));
    }

    [PublicAPI]
    public BetaTest AddAsyncTest()
    {
        return Test(
            from a in Gather(() => Task.FromResult(1))
            from b in Gather(() => 2)
            let c = Apply(async () => (await a) + b)
            select c.ShouldBe(3));
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