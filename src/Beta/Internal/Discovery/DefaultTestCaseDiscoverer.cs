using System.Reflection;
using Beta.Sdk.Abstractions;
using Beta.Sdk.Interfaces;

namespace Beta.Internal.Discovery;

/// <summary>
///     Defines the default test case discoverer.
/// </summary>
public class DefaultTestCaseDiscoverer : ITestCaseDiscoverer
{
    private readonly ITestSuiteActivator _activator;
    private readonly ILogger _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultTestCaseDiscoverer" /> class.
    /// </summary>
    /// <param name="activator">The test case activator to use.</param>
    /// <param name="logger">The logger to use.</param>
    public DefaultTestCaseDiscoverer(ITestSuiteActivator activator, ILogger logger)
    {
        _activator = activator;
        _logger = logger.CreateScope("testcase-discoverer");
    }

    /// <inheritdoc />
    public IEnumerable<Test> Discover(MethodInfo method)
    {
        var instance = _activator.Create(method.DeclaringType!);

        if (instance == null)
        {
            _logger.Warn(
                $"Skipping method [{method.DeclaringType?.Name}] because an instance of the class could not be created.");
            yield break;
        }

        _logger.Debug($"Checking method [{method.Name}] for tests.");

        var test = (BetaTest)method.Invoke(instance, [])!;
        var enumerator = test.Input != null
            ? EnumerateParameterizedTests(_activator, method, test)
            : EnumerateSingleTest(instance, method, test);

        foreach (var discoveredTest in enumerator)
        {
            _logger.Debug($"Discovered test [{discoveredTest.FriendlyName}].");
            yield return discoveredTest;
        }
    }

    private static IEnumerable<Test> EnumerateSingleTest(TestSuite instance,
                                                         MethodInfo method,
                                                         BetaTest test)
    {
        yield return new Test(instance, method, () => test.Apply(null!))
        {
            FriendlyName = test.TestName
        };
    }

    private static IEnumerable<Test> EnumerateParameterizedTests(ITestSuiteActivator activator,
                                                                 MethodInfo method,
                                                                 BetaTest test)
    {
        var index = 0;

        return from input in test.Input
               let testCaseInstance = activator.Create(method.DeclaringType!)
               where testCaseInstance is not null
               let testCase = (BetaTest?)method.Invoke(testCaseInstance, [])
               where testCase is not null
               select new Test(testCaseInstance, method, () => testCase.Apply(input))
               {
                   FriendlyName = $"{testCase.TestName}({input})",
                   Input = input.ToString(), // TODO: Use a better way to serialize this.
                   InputIndex = index++
               };
    }
}
