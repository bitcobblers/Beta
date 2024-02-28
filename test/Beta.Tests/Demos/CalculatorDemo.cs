using JetBrains.Annotations;
using static Beta.CommonSteps;

namespace Beta.Tests.Demos;

public class CalculatorDemo : TestContainer
{
    [PublicAPI]
    public BetaTest AddTestNoInput()
    {
        return Test(from a in Gather(() => 1)
                    let r = Apply(() => a + 2)
                    select r.IsEqual(3));
    }

    [PublicAPI]
    public BetaTest AddTestWithInput()
    {
        return Test(AdditionInput, i =>
            from a in Gather(() => 1)
            from b in Gather(2)
            let c = Apply(() => a + b)
            select c.IsEqual(i.Expected));
    }

    [PublicAPI]
    public BetaTest AddAsyncTest()
    {
        return Test(
            from a in Gather(() => Task.FromResult(1))
            from b in Gather(() => 2)
            let c = Apply(async () => (await a) + b)
            select c.IsEqual(3));
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