using System.Reflection;

namespace Beta.Discovery;

public class DefaultTestCaseDiscoverer(ITestContainerActivator activator) : ITestCaseDiscoverer
{
    /// <inheritdoc />
    public IEnumerable<Test> Discover(MethodInfo method)
    {
        var instance = activator.Create(method.DeclaringType!);
        var test = (BetaTest)method.Invoke(instance, [])!;

        if (test.Input != null)
        {
            return from input in test.Input
                   let testCaseInstance = activator.Create(method.DeclaringType!)
                   let testCase = (BetaTest)method.Invoke(testCaseInstance, [])!
                   select new Test
                   {
                       TestClassName = method.DeclaringType!.FullName!,
                       Instance = testCaseInstance,
                       Method = method,
                       Input = string.Empty, // <-- Need to serialize from input
                       Apply = () => testCase.Apply(input)
                   };
        }

        return new[]
        {
            new Test
            {
                TestClassName = method.DeclaringType!.FullName!,
                Instance = instance,
                Method = method,
                Apply = () => test.Apply(null!)
            }
        };
    }
}
