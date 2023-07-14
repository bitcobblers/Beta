namespace Beta.Tests.Demos;
public class DemoTests
{
    [Fact]
    public void RunCalculator()
    {
        var calcDemo = new CalculatorDemo();

        calcDemo.Execute();
    }
}
