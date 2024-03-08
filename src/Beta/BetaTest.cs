using System.Reflection;

namespace Beta;

[PublicAPI]
public record BetaTest(object Instance, object? Input, string Name, Func<Proof> Proof)
{
    public Guid Id { get; } = Guid.NewGuid();
    public Assembly Assembly => Instance.GetType().Assembly;

    public MethodBase? Method { get; set; }
}
