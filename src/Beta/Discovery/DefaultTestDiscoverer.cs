using System.Reflection;

namespace Beta.Discovery;

public class DefaultTestDiscoverer(ITestContainerActivator activator) : ITestDiscoverer
{
    /// <inheritdoc />
    public bool IsSuite(Type type) =>
        type.IsAssignableTo(typeof(TestContainer)) &&
        type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, []) != null &&
        type.GetMethods().Any(IsTest);

    /// <inheritdoc />
    public IEnumerable<BetaTest> Discover(Type type) =>
        from method in type.GetMethods()
        where IsTest(method)
        let instance = activator.Create(type)
        let test = method.Invoke(instance, null) as BetaTest
        where test is not null
        select test with
        {
            Method = method
        };

    private static bool IsTest(MethodInfo method) =>
        method is { IsPublic: true, IsStatic: false } &&
        method.GetParameters().Length == 0 &&
        method.ReturnType.IsAssignableTo(typeof(BetaTest));
}
