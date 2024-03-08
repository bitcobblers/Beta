using System.Reflection;

namespace Beta;

[PublicAPI]
public record BetaTest(TestContainer Instance, string TestName, Func<Proof> Proof)
{
    public Guid Id { get; } = Guid.NewGuid();
    public Assembly Assembly => Instance.GetType().Assembly;
    public string? DeclaringType => Instance.GetType().FullName;
    public string? MethodName => Method?.Name;

    public MethodBase? Method { get; set; }
    public object? Input { get; }

    protected BetaTest(TestContainer instance, object? input, string testName, Func<Proof> proof)
        : this(instance, testName, proof)
    {
        Input = input;
    }
}

public record BetaTest<TInput> : BetaTest
{
    public BetaTest(TestContainer container, TInput input, string testName, Func<TInput, Proof> apply)
        : base(container, input, testName, () => apply(input))
    {
    }
}
