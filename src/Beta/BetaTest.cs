using System.Reflection;

namespace Beta;

[PublicAPI]
public record BetaTest(TestContainer Instance, string Name, Func<Proof> Proof)
{
    public Guid Id { get; } = Guid.NewGuid();
    public Assembly Assembly => Instance.GetType().Assembly;

    public MethodBase? Method { get; set; }
}

public record BetaTest<TInput> : BetaTest
{
    public BetaTest(TestContainer container, TInput input, string name, Func<TInput, Proof> apply)
        : base(container, name, () => apply(input))
    {
    }
}
