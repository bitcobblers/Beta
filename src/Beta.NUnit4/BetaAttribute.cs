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

    public BetaAttribute()
    {
        Debugger.Launch();
        Debugger.Break();
    }

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
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public TestCommand Wrap(TestCommand command)
    {
        return new BehaviorCommand(command.Test);
    }
}
