using Beta.Tests.Demos;

namespace Beta.Tests;
public class BetaTestTests
{
    [Fact]
    public void RunCalculator()
    {
        var calcDemo = new CalculatorDemo();

        calcDemo.Execute();
    }
}
