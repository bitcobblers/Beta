using System.Diagnostics;
using Beta.NUnit4;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Commands;

// ReSharper disable once CheckNamespace
namespace Beta;

[PublicAPI]
[AttributeUsage(AttributeTargets.Method)]
public sealed class BetaAttribute : NUnitAttribute, IWrapTestMethod, ITestBuilder, IApplyToTest, IImplyFixture
{
    private readonly NUnitTestCaseBuilder _builder = new();

    public string? Feature { get; set; }

    /// <inheritdoc />
    public void ApplyToTest(Test test)
    {
        if (string.IsNullOrWhiteSpace(Feature) == false)
        {
            test.Properties.Add(PropertyNames.Category, Feature);
        }
    }

    /// <inheritdoc />
    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        // System.Diagnostics.Debugger.Launch();


        var clsInfo = method.MethodInfo.DeclaringType;
        var isContainer = clsInfo?.IsAssignableTo(typeof(TestContainer)) ?? false;
        var returnsBeta = method.MethodInfo.ReturnType == typeof(BetaTest);
        var instance = clsInfo!.GetConstructor(Type.EmptyTypes)?.Invoke(null);

        if (!isContainer || !returnsBeta || instance == null)
        {
            yield break;
        }

        if (method.MethodInfo.Invoke(instance, null) is not BetaTest betaTest)
        {
            yield break;
        }

        if (betaTest.Input == null)
        {
            yield return _builder.BuildTestMethod(method, suite, null);
        }
        else
        {
            foreach (var input in betaTest.Input)
            {
                var parameters = new TestCaseParameters(new[] { input });
                yield return _builder.BuildTestMethod(method, suite, parameters);

                //var testName = $"{betaTest.TestName}({input})";
                //var test = _builder.BuildTestMethod(method, suite, new object[] { input });
                //test.Name = testName;
                //yield return test;
            }
        }
    }

    /// <inheritdoc />
    public TestCommand Wrap(TestCommand command)
    {
        return new BehaviorCommand(command.Test);
    }
}
