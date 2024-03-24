using System.Reflection;
using Beta.Sdk.Abstractions;
using Beta.Sdk.Interfaces;

namespace Beta.Internal.Discovery;

/// <summary>
///     Defines the default test case discoverer.
/// </summary>
/// <param name="activator"></param>
public class DefaultTestCaseDiscoverer(ITestSuiteActivator activator) : ITestCaseDiscoverer
{
    /// <inheritdoc />
    public IEnumerable<Test> Discover(MethodInfo method)
    {
        var instance = activator.Create(method.DeclaringType!);
        var test = (BetaTest)method.Invoke(instance, [])!;

        if (test.Input != null)
        {
            return from input in test.Input
                   let testCaseInstance = activator.Create(method.DeclaringType!) as TestSuite
                   where testCaseInstance is not null
                   let testCase = (BetaTest)method.Invoke(testCaseInstance, [])!
                   select new Test(testCaseInstance, method, () => testCase.Apply(input))
                   {
                       Input = input.ToString() // TODO: Use a better way to serialize this.
                   };
        }

        return new[]
        {
            new Test(instance, method, () => test.Apply(null!))
            // {
            //     TestClassName = method.DeclaringType!.FullName!,
            //     Instance = instance,
            //     Method = method,
            //     Apply = () => test.Apply(null!)
            // }
        };
    }
}
