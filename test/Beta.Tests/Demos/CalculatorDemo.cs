﻿using System.Diagnostics.CodeAnalysis;
using Beta.Shouldly.ShouldBe;
using Microsoft.Extensions.DependencyInjection;

namespace Beta.Tests.Demos;

[PublicAPI]
public class CalculatorDemo : TestSuite.DI
{
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

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<Calculator>();
    }

    public BetaTest AdditionTest() =>
        Test(() =>
            from calculator in Require<Calculator>()
            let result = Apply(() => calculator.Add(1, 2))
            select result.ShouldBe(3));

    public BetaTest AddTestMany() =>
        Test(AdditionInput, i =>
            from calculator in Gather(() => Task.FromResult(new Calculator()))
            let result = Apply(async () => (await calculator).Add(i.A, i.B))
            select result.ShouldBe(i.Expected));

    // ---

    private record Input(int A, int B, int Expected);

    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public class Calculator
    {
        public int Add(int a, int b) => a + b;
    }
}
