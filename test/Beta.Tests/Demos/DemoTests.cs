namespace Beta.Tests.Demos;
public class DemoTests
{
    [Fact]
    public void RunCalculator()
    {
        var calcDemo = new CalculatorDemo();
        var proof = calcDemo.AddTestNoInput();

        proof.Prove();
    }
}
