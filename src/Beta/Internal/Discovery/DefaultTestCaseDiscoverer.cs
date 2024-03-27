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

        if (instance == null)
        {
            return [];
        }

        var test = (BetaTest)method.Invoke(instance, [])!;

        if (test.Input != null)
        {
            return from input in test.Input
                   let testCaseInstance = activator.Create(method.DeclaringType!)
                   where testCaseInstance is not null
                   let testCase = (BetaTest?)method.Invoke(testCaseInstance, [])
                   where testCase is not null
                   select new Test(testCaseInstance, method, () => testCase.Apply(input))
                   {
                       FriendlyName = $"{testCase.TestName}({input})",
                       Input = input.ToString() // TODO: Use a better way to serialize this.
                   };
        }

        return new[]
        {
            new Test(instance, method, () => test.Apply(null!))
            {
                FriendlyName = test.TestName
            }
        };
    }
}
