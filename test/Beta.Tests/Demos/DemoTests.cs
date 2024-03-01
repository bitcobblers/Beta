namespace Beta.Tests.Demos;

public class DemoTests
{
    [Fact]
    public async Task RunCalculator()
    {
        var calcDemo = new CalculatorDemo();
        var tests = calcDemo.AddTestMany();

        calcDemo.Prepare();

        foreach (var test in tests)
        {
            await test.Prove();
        }
    }
}
