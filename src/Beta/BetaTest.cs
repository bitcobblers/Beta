using System.Reflection;

namespace Beta;

[PublicAPI]
public record BetaTest(TestContainer Container, IEnumerable<object>? Input, string TestName, Func<object, Proof> Apply)
{
    public BetaTest(TestContainer container, string testName, Func<Proof> apply)
        : this(container, null, testName, _ => apply())
    {
    }

    public Assembly Assembly => Container.GetType().Assembly;
    public string? DeclaringType => Container.GetType().FullName;
    public string? MethodName => Method?.Name;

    public MethodBase? Method { get; set; }
}

public record BetaTest<TInput> : BetaTest
{
    public BetaTest(TestContainer container, IEnumerable<TInput> input, string testName, Func<TInput, Proof> apply)
        : base(container, (IEnumerable<object>)input, testName, o => apply((TInput)o))
    {
    }
}
