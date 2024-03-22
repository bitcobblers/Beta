using System.Reflection;

namespace Beta.Discovery;

/// <summary>
///     Defines the default test suite discoverer
/// </summary>
/// <param name="testCaseDiscoverer"></param>
public class DefaultTestDiscoverer(ITestCaseDiscoverer testCaseDiscoverer)
    : ITestDiscoverer
{
    /// <inheritdoc />
    public bool IsSuite(Type type) =>
        type.IsAssignableTo(typeof(TestContainer)) &&
        type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, []) != null &&
        type.GetMethods().Any(IsTest);

    /// <inheritdoc />
    public IEnumerable<Test> Discover(Type type) =>
        from method in type.GetMethods()
        where IsTest(method)
        from test in testCaseDiscoverer.Discover(method)
        select test;

    private static bool IsTest(MethodInfo method) =>
        method is { IsPublic: true, IsStatic: false } &&
        method.GetParameters().Length == 0 &&
        method.ReturnType.IsAssignableTo(typeof(BetaTest));
}